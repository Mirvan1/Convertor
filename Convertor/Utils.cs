using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Convertor.Dtos;
using System.Reflection;
using System.Collections.ObjectModel;
using Convertor.Exceptions;

namespace Convertor;
internal class Utils
{
    internal static List<TableDto> ConvertToDto(string connectionString, bool includeRelations = true, params string[] excludeTables)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DataTable dataTable = new DataTable();

                List<TableDto> tableDtos = new List<TableDto>();

                
                using (SqlDataAdapter adapter = new SqlDataAdapter(Constants.Query, connection))
                {
                    adapter.Fill(dataTable);
                }

                if (dataTable.Rows.Count > 0)
                {
                    var groupByTable = dataTable
                        .AsEnumerable()
                        .GroupBy(row => row["TABLE_NAME"])
                        .Where(group => !excludeTables.Contains(group.Key));


                    foreach (var table in groupByTable)
                    {
                        TableDto tableDto = new()
                        {
                            TableName = table.Key.ToString(),
                            Columns = new List<ColumnDto>(),
                            RelationalTables = new List<string>()
                        };
                        foreach (var column in table)
                        {
                            ColumnDto columnDto = new();
                            try
                            {
                                if (tableDto.Columns.Count == 0
                                   || (tableDto.Columns.Count > 0 && tableDto.Columns.All(x =>!string.IsNullOrEmpty(x.Name) && !x.Name.Trim().Equals(column["COLUMN_NAME"].ToString().Trim()))))
                                {

                                    columnDto.Name = column["COLUMN_NAME"].ToString();
                                    columnDto.DataType = column["DATA_TYPE"].ToString();
                                    columnDto.IsNullable = column["IS_NULLABLE"].ToString().Equals("YES") ? true : false;
                                }
                            }catch(Exception e)
                            {
                                throw;
                            }
                            if (includeRelations
                                && !string.IsNullOrWhiteSpace(column["FK_TABLE_NAME"].ToString())
                                && !column["FK_TABLE_NAME"].ToString().Equals(table.Key)
                                && tableDtos.Any(table => table.TableName.Equals(column["FK_TABLE_NAME"].ToString()))

                                )
                            {
                                    var relationalTables=tableDtos.Where(x => x.TableName.Equals(column["FK_TABLE_NAME"].ToString())).ToList();

                                    if(relationalTables!=null 
                                    && relationalTables.Count > 0
                                    )
                                    {
                                        foreach( var relationalTable in relationalTables)
                                        {
                                            ColumnDto relationalColumnDto = new()
                                            {
                                                Name = tableDto.TableName,
                                                DataType = tableDto.TableName,
                                                IsNullable = false
                                            };
                                            relationalTable.Columns.Add(relationalColumnDto);
                                        }
                                    }


                                    tableDto.RelationalTables.Add(column["FK_TABLE_NAME"].ToString());
                            }
                            tableDto.Columns.Add(columnDto);
                        }

                        tableDtos.Add(tableDto);
                    }
                }


                connection.Close();
                return tableDtos;
            }
        }
        catch (Exception e)
        {
            throw new ConvertorException(e.Message, e);
        }
    }
    internal static string GenerateCsharpDto(TableDto table, bool includeRelations = true)
    {
        StringBuilder generateBuilder = new StringBuilder();
        generateBuilder.Append($"public class {table.TableName}{Environment.NewLine}{Constants.OpenCurlyParanthesis}{Environment.NewLine}");

        if (table.Columns != null && table.Columns.Count > 0)
        {
            foreach (var column in table.Columns)
            {
                if (!string.IsNullOrEmpty(column.Name))
                {
                    string nullParam = column.IsNullable ? "?" : string.Empty;
                    generateBuilder.Append($"\tpublic {SqlToCSharpType(column.DataType)}{nullParam} {column.Name};{Environment.NewLine}");
                }
                }

            if (includeRelations && table.RelationalTables != null && table.RelationalTables.Count > 0)
                table.RelationalTables.ForEach(relation => generateBuilder.Append($"\tpublic ICollection<{relation}> {relation};{Environment.NewLine}"));
        }
        generateBuilder.Append($"{Constants.ClosedCurlyParanthesis}{Environment.NewLine}");
        return generateBuilder.ToString();
    }

    internal static string GenerateTSDto(TableDto table, bool includeRelations = true)
    {
        StringBuilder generateBuilder = new StringBuilder();
        generateBuilder.Append($"export interface {table.TableName}{Environment.NewLine}{Constants.OpenCurlyParanthesis}{Environment.NewLine}");

        if (table.Columns != null && table.Columns.Count > 0)
        {
            foreach (var column in table.Columns)
            {
                if (!string.IsNullOrEmpty(column.Name))
                {
                    string nullParam = column.IsNullable ? "?" : string.Empty;
                    generateBuilder.Append($"\t{TitleCase(column.Name)}{nullParam}:{SqlToTsType(column.DataType)};{Environment.NewLine}");
                }
            }

            if (includeRelations && table.RelationalTables != null && table.RelationalTables.Count > 0)
                table.RelationalTables.ForEach(relation => generateBuilder.Append($"\t{TitleCase(relation)}: {relation}[];{Environment.NewLine}"));
        }
        generateBuilder.Append($"{Constants.ClosedCurlyParanthesis}{Environment.NewLine}");
        return generateBuilder.ToString();
    }

    internal static string GenerateTSFromAssembly(Type type)
    {
        StringBuilder tsDto = new StringBuilder();
        tsDto.Append($"export interface {type.Name}{Constants.OpenCurlyParanthesis}{Environment.NewLine}");

        foreach (var prop in type.GetProperties())
        {
            string tsType = Utils.CSToTSType(prop);
            tsDto.AppendLine($"\t {prop.Name}:{tsType}");
        }
        tsDto.AppendLine(Constants.ClosedCurlyParanthesis);
        return tsDto.ToString();
    }


    private static string SqlToCSharpType(string type)
    {
        if (string.IsNullOrEmpty(type))
            throw new ConvertorException("sqlType is null or empty");


        if (type.Contains("List") || type.Contains("ICollection") || type.Contains("IEnumerable")) return type;


        switch (type.ToLower())
        {
            case "bigint":
                return "long";
            case "binary":
            case "image":
            case "varbinary":
            case "timestamp":
                return "byte[]";
            case "bit":
                return "bool";
            case "char":
            case "nchar":
            case "ntext":
            case "nvarchar":
            case "text":
            case "varchar":
                return "string";
            case "datetime":
            case "smalldatetime":
            case "date":
            case "time":
                return "DateTime";
            case "decimal":
            case "money":
            case "smallmoney":
            case "numeric":
                return "decimal";
            case "float":
                return "double";
            case "int":
                return "int";
            case "real":
                return "float";
            case "uniqueidentifier":
                return "Guid";
            case "smallint":
                return "short";
            case "tinyint":
                return "byte";
            case "varbinary(max)":
            case "varchar(max)":
                return "byte[]";
            case "xml":
                return "XmlDocument";
            default:
                return type;
        }
    }


    private static string SqlToTsType(string type)
    {
        if (string.IsNullOrEmpty(type))
            throw new ConvertorException("sqlType is null or empty");

        if (type.Contains("List") || type.Contains("ICollection") || type.Contains("IEnumerable"))
        {
            type = type.Replace("ICollection", "").Replace("List", "").Replace("IEnumerable", "").Replace("<", "").Replace(">", "");
            return type+"[]";
        }


        switch (type.ToLower())
        {
            case "bigint":
                return "number";
            case "binary":
            case "image":
            case "varbinary":
            case "timestamp":
                return "Blob";  
            case "bit":
                return "boolean";
            case "char":
            case "nchar":
            case "ntext":
            case "nvarchar":
            case "text":
            case "varchar":
                return "string";
            case "datetime":
            case "smalldatetime":
            case "date":
            case "time": 
            case "decimal":
            case "money":
            case "smallmoney":
            case "numeric":
            case "float":
            case "int":
            case "real":
            case "smallint":
            case "tinyint":
                return "number";
            case "uniqueidentifier":
                return "string"; 
            case "varbinary(max)":
            case "varchar(max)":
                return "string"; 
            case "xml":
                return "string";  
            default:
                return type; 
        }
    }


    private static string CSToTSType(PropertyInfo actualType)
    {
 
        try
        {
            string nullableSuffix = string.Empty;

            if (actualType.PropertyType.IsGenericType && actualType.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                nullableSuffix = " | null";
            }

            if (IsGenericList(actualType.PropertyType))
            {
                Type itemType = actualType.PropertyType.GetGenericArguments()[0];

                return $"{itemType.Name}[]{nullableSuffix}";
            }
             switch (actualType.PropertyType.Name)
            {
                case "Int16":
                case "Int32":
                case "Int64":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                case "Single":
                case "Double":
                case "Decimal":
                    return "number" + nullableSuffix;

                case "String":
                    return "string" + nullableSuffix;

                case "Boolean":
                    return "boolean" + nullableSuffix;

                case "DateTime":
                    return "Date" + nullableSuffix;

                case "Guid":
                    return "string" + nullableSuffix;

                default:
                    if (actualType.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null)
                    {
                        CSToTSType(actualType);
                     }

                    if (actualType.PropertyType.IsArray)
                    {
                        CSToTSType(actualType);
                     }
                    return actualType.Name + nullableSuffix;
            }

        }
        catch (Exception e)
        {
            throw new ConvertorException(e.Message, e);
        }
    }



    internal static string TitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        if (input.Length == 1)
            return input.ToLower();

        return char.ToLower(input[0]) + input.Substring(1);
    }
    private static bool IsGenericList(Type type) => type.IsGenericType &&
           (type.GetGenericTypeDefinition() == typeof(List<>) ||
            type.GetGenericTypeDefinition() == typeof(Collection<>));
}

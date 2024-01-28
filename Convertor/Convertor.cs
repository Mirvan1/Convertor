using Convertor.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convertor;

    public class Convertor
{

    public void SqlToCsharp(string path, string connectionString, bool includeRelations = true, params string[] excludeTables) =>
            File.WriteAllText(path, SqlToCsharpInStr(connectionString, includeRelations = true, excludeTables));

    public byte[] SqlToCsharp(string connectionString, bool includeRelations = true, params string[] excludeTables) =>
        Encoding.ASCII.GetBytes(SqlToCsharpInStr(connectionString, includeRelations = true, excludeTables));


    public string SqlToCsharpInStr(string connectionString, bool includeRelations = true, params string[] excludeTables)
    {
        try
        {
            List<TableDto> tableDtos = Utils.ConvertToDto(connectionString, includeRelations, excludeTables);
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);

            if (tableDtos != null && tableDtos.Count > 0)
            {
                StringBuilder generatedCode = new StringBuilder();
                generatedCode.Append(Constants.LibraryTitle);
                generatedCode.Append($"namespace {builder.InitialCatalog ?? "GeneratedDtos"};\n\n");
                foreach (var table in tableDtos)
                {
                    generatedCode.Append(Utils.GenerateCsharpDto(table));
                }
                return generatedCode.ToString();
            }
            return string.Empty;
        }
        catch (Exception e)
        {
            throw;
        }
    }


    public void SqlToTs(string path,string connectionString, bool includeRelations = true, params string[] excludeTables) =>
              File.WriteAllText(path, SqlToTsInStr(connectionString, includeRelations = true, excludeTables));

    public byte[] SqlToTs(string connectionString, bool includeRelations = true, params string[] excludeTables)=>
        Encoding.ASCII.GetBytes(SqlToTsInStr(connectionString,includeRelations = true, excludeTables));
    public string SqlToTsInStr(string connectionString, bool includeRelations = true, params string[] excludeTables)
    {
        try
        {
            List<TableDto> tableDtos = Utils.ConvertToDto(connectionString, includeRelations, excludeTables);
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);

            if (tableDtos != null && tableDtos.Count > 0)
            {
                StringBuilder generatedCode = new StringBuilder();
                generatedCode.Append(Constants.LibraryTitle);

                foreach (var table in tableDtos)
                {
                    generatedCode.Append(Utils.GenerateTSDto(table));
                }
                return generatedCode.ToString();
            }
            return string.Empty;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public void CSharpToTs(string path, Assembly assembly) =>
        File.WriteAllText(path, CsharpToTsInStr(assembly));

    public byte[] CSharpToTs(Assembly assembly) =>
        Encoding.ASCII.GetBytes(CsharpToTsInStr(assembly));

    public string CsharpToTsInStr(Assembly assembly)
    {
        try
        {
            StringBuilder generatedCode = new StringBuilder();
            generatedCode.Append(Constants.LibraryTitle);

            foreach (var type in assembly.GetTypes())
            {
                if (type.GetProperties() is not null)
                    generatedCode.AppendLine(Utils.GenerateTSFromAssembly(type));
            }
            return generatedCode.ToString();
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

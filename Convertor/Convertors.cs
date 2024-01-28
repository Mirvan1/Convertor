using Convertor.Dtos;
using Convertor.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convertor;

public class Convertors
{

    public static void SqlToCSharp(string path, string filename, string connectionString, bool includeRelations = true, params string[] excludeTables)
    {

        if (string.IsNullOrEmpty(path))
            throw new ConvertorException("The path variable cannot be empty");

        if (string.IsNullOrEmpty(filename))
            throw new ConvertorException("The filename cannot be empty");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (!filename.EndsWith(".cs"))
            filename += ".cs";
        try
        {
            File.WriteAllText(Path.Combine(path,filename), SqlToCSharpStr(connectionString, includeRelations = true, excludeTables));
        }
        catch (Exception e)
        {
            throw new ConvertorException(e.Message, e);
        }
    }

    public static byte[] SqlToCSharp(string connectionString, bool includeRelations = true, params string[] excludeTables) =>
        Encoding.ASCII.GetBytes(SqlToCSharpStr(connectionString, includeRelations = true, excludeTables));


    public static string SqlToCSharpStr(string connectionString, bool includeRelations = true, params string[] excludeTables)
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
            throw new ConvertorException(e.Message, e);
        }
    }


    public static void SqlToTypescript(string path, string filename, string connectionString, bool includeRelations = true, params string[] excludeTables)
    {
        if (string.IsNullOrEmpty(path))
            throw new ConvertorException("The path variable cannot be empty");

        if (string.IsNullOrEmpty(filename))
            throw new ConvertorException("The filename cannot be empty");

        if (!Directory.Exists(path))
               Directory.CreateDirectory(path);

        if (!filename.EndsWith(".ts"))
            filename += ".ts";
        try
        {
            File.WriteAllText(Path.Combine(path,filename), SqlToTypescriptStr(connectionString, includeRelations = true, excludeTables));
        }
        catch (Exception e)
        {
            throw new ConvertorException(e.Message, e);
        }
    }

    public static byte[] SqlToTypescript(string connectionString, bool includeRelations = true, params string[] excludeTables) =>
        Encoding.ASCII.GetBytes(SqlToTypescriptStr(connectionString, includeRelations = true, excludeTables));
    public static string SqlToTypescriptStr(string connectionString, bool includeRelations = true, params string[] excludeTables)
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
            throw new ConvertorException(e.Message, e);

        }
    }

    public static void CSharpToTypescript(string path,string filename, Assembly assembly)
    {
        if (string.IsNullOrEmpty(path))
            throw new ConvertorException("The path variable cannot be empty");

        if (string.IsNullOrEmpty(filename))
            throw new ConvertorException("The filename cannot be empty");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (!filename.EndsWith(".ts"))
            filename += ".ts";
        try
        {
            File.WriteAllText(Path.Combine(path,filename), CsharpToTypescriptStr(assembly));
        }catch(Exception e)
        {
            throw new ConvertorException(e.Message, e);
        }
        }


    public static void CSharpToTypescriptsDll(string dllPath,string path, string filename, Assembly assembly)
    {
        if (string.IsNullOrEmpty(path))
            throw new ConvertorException("The path variable cannot be empty");

        if (string.IsNullOrEmpty(filename))
            throw new ConvertorException("The filename cannot be empty");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (!filename.EndsWith(".ts"))
            filename += ".ts";
        try
        {
            File.WriteAllText(Path.Combine(path, filename), CsharpToTypescriptDll(dllPath));
        }
        catch (Exception e)
        {
            throw new ConvertorException(e.Message, e);
        }
    }


    public static byte[] CSharpToTypescript(Assembly assembly) =>
        Encoding.ASCII.GetBytes(CsharpToTypescriptStr(assembly));

    public static byte[] CSharpToTypescriptDll(string dllPath) =>
     Encoding.ASCII.GetBytes(CsharpToTypescriptDll(dllPath));

    public static string CsharpToTypescriptDll(string dllPath)
    {
        if (string.IsNullOrWhiteSpace(dllPath))
            throw new ConvertorException("The file cannot be empty");

        if (!dllPath.EndsWith("dll"))
            throw new ConvertorException("The file must be in dll format");

        try
        {
            Assembly assembly = Assembly.LoadFile(dllPath);
            return CsharpToTypescriptStr(assembly);
        }
        catch(Exception e)
        {
            throw new ConvertorException(e.Message, e);
        }
    }

    public static string CsharpToTypescriptStr(Assembly assembly)
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
            throw new ConvertorException(e.Message, e);
        }
    }
}

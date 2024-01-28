using Convertor;
using Convertor.ConsoleExamples;
using System.Reflection;


Assembly assembly = typeof(ExampleDtos).Assembly;

byte[] ex1 = Convertors.SqlToCSharp("Data Source=DESKTOP-HGKR5P9\\MSSQLSERVERDB;Initial Catalog=ExampleDB;Integrated Security=True;Encrypt=false;", true, "sysdiagrams");
byte[] ex2 = Convertors.SqlToTypescript("Data Source=DESKTOP-HGKR5P9\\MSSQLSERVERDB;Initial Catalog=ExampleDB;Integrated Security=True;Encrypt=false;", true, "sysdiagrams");

Convertors.SqlToTypescript(@"C:/Users/Projects/Convertor/Convertor/Convertor.ConsoleExamples//", "exampleTs", "Data Source=DESKTOP-HGKR5P9\\MSSQLSERVERDB;Initial Catalog=ExampleDB;Integrated Security=True;Encrypt=false;", true, "sysdiagrams");
Convertors.SqlToCSharp(@"C:/Users/Projects/Convertor/Convertor/Convertor.ConsoleExamples/", "exampleCs", "Data Source=DESKTOP-HGKR5P9\\MSSQLSERVERDB;Initial Catalog=ExampleDB;Integrated Security=True;Encrypt=false;", true, "sysdiagrams");


string str1 = Convertors.SqlToCSharpStr("Data Source=DESKTOP-HGKR5P9\\MSSQLSERVERDB;Initial Catalog=ExampleDB;Integrated Security=True;Encrypt=false;", true, "sysdiagrams");
string str2 = Convertors.SqlToTypescriptStr("Data Source=DESKTOP-HGKR5P9\\MSSQLSERVERDB;Initial Catalog=ExampleDB;Integrated Security=True;Encrypt=false;", true, "sysdiagrams");
Console.WriteLine(str1);
Console.WriteLine(str2);

string str3 = Convertors.CsharpToTypescriptStr(assembly);
byte[] ex3 = Convertors.CSharpToTypescript(assembly);
Convertors.CSharpToTypescript("C:\\Users\\Lenovo\\source\\repos\\Convertor\\Convertor.ConsoleTest\\", "ts2example", assembly);
 
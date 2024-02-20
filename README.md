# Convertor
Convertor is a NuGet library designed to convert SQL tables into C# and TypeScript DTOs and C# entities to TypeScript. It simplifies the development process by automating the conversion between these different formats.

## Examples and how to use:

### Convert SQL tables to C#:
```
byte[] example1 = Convertors.SqlToCSharp("connection-string");
```
### Convert SQL tables to TypeScript:
``` 
byte[] example2 = Convertors.SqlToTypescript("connection-string");
```
### Save TypeScript conversion to a file:
``` 
Convertors.SqlToTypescript("path-to-save", "filename.ts", "connection-string");
```
### Save C# conversion to a file:
``` 
Convertors.SqlToCSharp("path-to-save", "filename.cs", "connection-string");
```
### Convert C# entities to TypeScript:
``` 
string typescript = Convertors.CsharpToTypescriptStr(typeof(YourClass).Assembly);
```

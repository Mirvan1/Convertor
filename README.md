# Convertor
Convertor is a NuGet library designed to convert SQL tables into C# and TypeScript DTOs and C# entities to TypeScript. It simplifies the development process by automating the conversion between these different formats.

## Examples and how to use:

###Convert SQL tables to C#:
```csharp
byte[] csharpDTO = Convertors.SqlToCSharp("fake-connection-string", true, "except-your-tables-name-optional");

###Convert SQL tables to TypeScript:
```csharp
byte[] typescriptDTO = Convertors.SqlToTypescript("fake-connection-string", true, "except-your-tables-name-optional");

###Save TypeScript conversion to a file:
```csharp
Convertors.SqlToTypescript("path-to-save", "filename.ts", "fake-connection-string", true, "your-table-name");

###Save C# conversion to a file:
```csharp
Convertors.SqlToCSharp("path-to-save", "filename.cs", "fake-connection-string", true, "your-table-name");

###Convert C# entities to TypeScript:
```csharp
string typescript = Convertors.CsharpToTypescriptStr(typeof(YourClass).Assembly);


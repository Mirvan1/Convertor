﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convertor;
    internal class Constants
    {
    public const string OpenCurlyParanthesis = "{";
    public const string ClosedCurlyParanthesis = "}";

    public const string OpenParanthesis = "(";
    public const string CloseParanthesis = ")";

    public const string LibraryTitle = $"/*\n\t<auto-generated>\n\t\tThis code generated from Convertor library\n\t</auto-generated>\n*/\n\n";

    public const string Query =@"SELECT 
    col.TABLE_SCHEMA, 
    col.TABLE_NAME, 
    col.COLUMN_NAME, 
    col.DATA_TYPE,
    col.IS_NULLABLE,
    CASE 
        WHEN fk.FK_TABLE_NAME IS NOT NULL AND fk.FK_TABLE_NAME = col.TABLE_NAME THEN 'Self-Referencing FK'
        WHEN fk.FK_TABLE_NAME IS NOT NULL THEN 'Foreign Key'
        ELSE 'No Relation'
    END AS RelationType,
    fk.FK_TABLE_SCHEMA,
    fk.FK_TABLE_NAME,
    fk.FK_COLUMN_NAME
FROM 
    INFORMATION_SCHEMA.COLUMNS col
LEFT JOIN 
    (SELECT 
         kcu1.TABLE_SCHEMA AS FK_TABLE_SCHEMA,
         kcu1.TABLE_NAME AS FK_TABLE_NAME,
         kcu1.COLUMN_NAME AS FK_COLUMN_NAME,
         rc.CONSTRAINT_NAME,
         kcu2.TABLE_SCHEMA AS REFERENCED_TABLE_SCHEMA,
         kcu2.TABLE_NAME AS REFERENCED_TABLE_NAME,
         kcu2.COLUMN_NAME AS REFERENCED_COLUMN_NAME
     FROM 
         INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc 
     INNER JOIN 
         INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu1 
         ON kcu1.CONSTRAINT_CATALOG = rc.CONSTRAINT_CATALOG 
         AND kcu1.CONSTRAINT_SCHEMA = rc.CONSTRAINT_SCHEMA 
         AND kcu1.CONSTRAINT_NAME = rc.CONSTRAINT_NAME
     INNER JOIN 
         INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu2 
         ON kcu2.CONSTRAINT_CATALOG = rc.UNIQUE_CONSTRAINT_CATALOG 
         AND kcu2.CONSTRAINT_SCHEMA = rc.UNIQUE_CONSTRAINT_SCHEMA 
         AND kcu2.CONSTRAINT_NAME = rc.UNIQUE_CONSTRAINT_NAME
     WHERE 
         NOT (kcu1.TABLE_NAME = kcu2.TABLE_NAME AND kcu1.COLUMN_NAME = kcu2.COLUMN_NAME)) fk
ON  col.TABLE_SCHEMA = fk.REFERENCED_TABLE_SCHEMA
    AND col.TABLE_NAME = fk.REFERENCED_TABLE_NAME
    AND col.COLUMN_NAME = fk.REFERENCED_COLUMN_NAME
ORDER BY 
    col.TABLE_SCHEMA, 
    col.TABLE_NAME, 
    col.COLUMN_NAME;";


}


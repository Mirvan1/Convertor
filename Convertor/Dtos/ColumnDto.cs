using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convertor.Dtos;
internal class ColumnDto
{
    public string Name { get; set; }
    public string DataType { get; set; }
    public bool IsNullable { get; set; }
}

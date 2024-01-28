using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convertor.Dtos;
 internal class TableDto
{
     public string TableName { get; set; }
    public List<string> RelationalTables { get; set; }

    public List<ColumnDto> Columns { get; set; }

}

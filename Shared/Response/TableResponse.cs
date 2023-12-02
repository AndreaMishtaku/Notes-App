using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Response;

public class TableResponse<T>
{
    public List<TableColumn> Columns { get; set; }
    public IEnumerable<T> Rows { get; set; }
    public int RowCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
    public int TotalPages { get; set; }
}
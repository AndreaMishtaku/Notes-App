using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Request;

public class LookupDTO
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Filter[] Filters { get; set; }

    public Sort Sort { get; set; }
}

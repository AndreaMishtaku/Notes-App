using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Request;

public class Sort
{
    public string Field { get; set; }
    public Order Order { get; set; }
}

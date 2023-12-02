using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Request;

public class Filter
{

    public string Field { get; set; }

    public Operation Operation { get; set; }

    public string Value { get; set; }
}

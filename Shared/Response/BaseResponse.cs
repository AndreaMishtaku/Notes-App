using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.Response;

public class BaseResponse
{
    public string? Message { get; set; }
    public int StatusCode { get; set; }
}

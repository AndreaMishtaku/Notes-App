using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions;

public sealed class BadRequestException : Exception
{
    public BadRequestException(string message)
    : base(message)
    {
    }
}
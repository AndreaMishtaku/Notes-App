using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO;

public class TokenDTO
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public TokenDTO(string accessToken,string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}

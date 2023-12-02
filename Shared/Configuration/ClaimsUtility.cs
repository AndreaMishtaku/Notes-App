using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace Shared.Configuration;
public interface IClaimsUtility
{
    int ReadCurrentUserId();
    string ReadCurrentUserEmail();
}

public class ClaimsUtility : IClaimsUtility
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClaimsUtility(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public int ReadCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.First(c => c.Type == "Id").Value;
     
        if (userIdClaim != null)
        {
            return Convert.ToInt32(userIdClaim);
        }

        return 0;
    }

    public string ReadCurrentUserEmail()
    {
        var emailClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email);
        return emailClaim?.Value;
    }
}
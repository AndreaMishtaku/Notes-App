using Entities;
using NotesApp.Shared.Configuration;
using NotesApp.Utility;

namespace NotesApp.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IJwtUtils jwtUtils)
    {
        var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var validateTokenResult = jwtUtils.ValidateToken(accessToken);

        if (validateTokenResult.Item1.HasValue &&
            !string.IsNullOrWhiteSpace(validateTokenResult.Item2))
        {
            var currentUser = new User();
            //await serviceManager.UserService.GetUserById(validateTokenResult.Item1.Value);
            context.Items["User"] = context.User;
        }
        await _next(context);
    }
}
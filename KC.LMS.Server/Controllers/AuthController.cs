using KC.LMS.Server.Services;
using KC.LMS.Storage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KC.LMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService, IJwtTokenService tokenService, ILogger<AuthController> logger)
    : ControllerBase
{
    public record AuthResponse(string Token, DateTimeOffset ExpiresAt, Guid UserId, string Email, string UserName);

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = await authService.RegisterAsync(request, cancellationToken);
        if (user is null)
        {
            return Conflict(new ProblemDetails { Title = "Registration failed. The tenant may not exist or the email/username is already taken." });
        }

        logger.LogInformation("User {UserId} registered in tenant {TenantId}", user.UserId, user.TenantId);
        var (token, expiresAt) = tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, expiresAt, user.UserId, user.Email, user.UserName));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await authService.LoginAsync(request, cancellationToken);
        if (user is null)
        {
            return Unauthorized(new ProblemDetails { Title = "Invalid credentials." });
        }

        var (token, expiresAt) = tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, expiresAt, user.UserId, user.Email, user.UserName));
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // JWT auth is stateless: the client discards the token. This endpoint exists for
        // symmetry and future token revocation (e.g., a denylist or refresh-token invalidation).
        return NoContent();
    }
}

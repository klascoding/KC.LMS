using KC.LMS.Service.Models;

namespace KC.LMS.Service;

public interface IAuthService
{
    /// <summary>Registers a new user in the tenant identified by slug. Returns null if the email/username is taken or the tenant does not exist.</summary>
    Task<AuthUserResult?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);

    /// <summary>Validates credentials. Returns null if the user is unknown, inactive, or the password does not match.</summary>
    Task<AuthUserResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}

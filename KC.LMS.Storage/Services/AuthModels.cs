namespace KC.LMS.Storage.Services;

public record RegisterRequest(string TenantSlug, string Email, string UserName, string Password, string? DisplayName);

public record LoginRequest(string TenantSlug, string Email, string Password);

public record AuthUserResult(Guid UserId, Guid TenantId, string Email, string UserName, string? DisplayName, IReadOnlyList<string> Roles);

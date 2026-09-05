using KC.LMS.Storage.Entities;

namespace KC.LMS.Service.Models;

public record RegisterRequest(string TenantSlug, string Email, string UserName, string Password, string? DisplayName);

public record LoginRequest(string TenantSlug, string Email, string Password);

/// <summary>An active role assignment. ScopeId is null for tenant-wide grants.</summary>
public record RoleGrant(string RoleName, AccessScopeType ScopeType, Guid? ScopeId);

public record AuthUserResult(Guid UserId, Guid TenantId, string Email, string UserName, string? DisplayName, IReadOnlyList<RoleGrant> Grants);

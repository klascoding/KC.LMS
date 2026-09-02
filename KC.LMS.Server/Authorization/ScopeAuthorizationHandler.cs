using System.Security.Claims;
using KC.LMS.Server.Services;
using KC.LMS.Storage.Entities;
using Microsoft.AspNetCore.Authorization;

namespace KC.LMS.Server.Authorization;

/// <summary>
/// Requires the user to hold <see cref="RoleName"/> either tenant-wide (role claim) or
/// scoped (scope claim) to the route value named <see cref="RouteValueName"/>.
/// </summary>
public class ScopeRequirement(string roleName, AccessScopeType scopeType, string routeValueName)
    : IAuthorizationRequirement
{
    public string RoleName { get; } = roleName;
    public AccessScopeType ScopeType { get; } = scopeType;
    public string RouteValueName { get; } = routeValueName;
}

public class ScopeAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        // Tenant-wide role satisfies any scope.
        if (context.User.IsInRole(RoleNames.TenantAdmin) || context.User.IsInRole(requirement.RoleName))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var routeValue = httpContextAccessor.HttpContext?.GetRouteValue(requirement.RouteValueName)?.ToString();
        if (!Guid.TryParse(routeValue, out var scopeId))
        {
            return Task.CompletedTask;
        }

        var expected = $"{requirement.RoleName}:{requirement.ScopeType}:{scopeId}";
        if (context.User.FindAll(JwtTokenService.ScopeClaim).Any(c => string.Equals(c.Value, expected, StringComparison.OrdinalIgnoreCase)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

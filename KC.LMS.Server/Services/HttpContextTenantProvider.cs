using KC.LMS.Storage;

namespace KC.LMS.Server.Services;

/// <summary>Resolves the current tenant from the authenticated user's JWT claims.</summary>
public class HttpContextTenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    public Guid? TenantId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User.FindFirst(JwtTokenService.TenantIdClaim)?.Value;
            return Guid.TryParse(claim, out var tenantId) ? tenantId : null;
        }
    }
}

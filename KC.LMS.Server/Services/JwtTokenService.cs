using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KC.LMS.Storage.Services;
using Microsoft.IdentityModel.Tokens;

namespace KC.LMS.Server.Services;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}

public interface IJwtTokenService
{
    (string Token, DateTimeOffset ExpiresAt) CreateToken(AuthUserResult user);
}

public class JwtTokenService(Microsoft.Extensions.Options.IOptions<JwtOptions> options) : IJwtTokenService
{
    public const string TenantIdClaim = "tenant_id";
    public const string ScopeClaim = "scope";

    private readonly JwtOptions _options = options.Value;

    public (string Token, DateTimeOffset ExpiresAt) CreateToken(AuthUserResult user)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiryMinutes);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(TenantIdClaim, user.TenantId.ToString()),
        ];

        foreach (var grant in user.Grants)
        {
            if (grant.ScopeType == KC.LMS.Storage.Entities.AccessScopeType.Tenant)
            {
                // Tenant-wide grants map to standard role claims for [Authorize(Roles = ...)].
                claims.Add(new Claim(ClaimTypes.Role, grant.RoleName));
            }
            else
            {
                // Scoped grants: "{role}:{scopeType}:{scopeId}", e.g. "OrgManager:Organization:<guid>".
                claims.Add(new Claim(ScopeClaim, $"{grant.RoleName}:{grant.ScopeType}:{grant.ScopeId}"));
            }
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

using KC.LMS.Storage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KC.LMS.Storage.Services;

public class AuthService(ApplicationDbContext dbContext, ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthUserResult?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .SingleOrDefaultAsync(t => t.Slug == request.TenantSlug && t.IsActive, cancellationToken);
        if (tenant is null)
        {
            logger.LogWarning("Registration attempted for unknown tenant {TenantSlug}", request.TenantSlug);
            return null;
        }

        var exists = await dbContext.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.TenantId == tenant.Id && (u.Email == request.Email || u.UserName == request.UserName), cancellationToken);
        if (exists)
        {
            return null;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Email = request.Email,
            UserName = request.UserName,
            DisplayName = request.DisplayName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTimeOffset.UtcNow,
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthUserResult(user.Id, user.TenantId, user.Email, user.UserName, user.DisplayName, []);
    }

    public async Task<AuthUserResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .IgnoreQueryFilters()
            .Include(u => u.UserAccesses)
            .SingleOrDefaultAsync(u => u.Tenant!.Slug == request.TenantSlug && u.Email == request.Email, cancellationToken);

        if (user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var roles = user.UserAccesses
            .Where(a => a.RevokedAt is null)
            .Select(a => a.Role)
            .Distinct()
            .ToList();

        return new AuthUserResult(user.Id, user.TenantId, user.Email, user.UserName, user.DisplayName, roles);
    }
}

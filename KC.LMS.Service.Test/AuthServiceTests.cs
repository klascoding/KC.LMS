using KC.LMS.Service.Models;
using KC.LMS.Storage;
using KC.LMS.Storage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace KC.LMS.Service.Test;

public sealed class AuthServiceTests : IDisposable
{
    private readonly SqliteDbFixture _fixture = new();

    public void Dispose() => _fixture.Dispose();

    private IAuthService CreateService(ApplicationDbContext context) =>
        new AuthService(context, NullLogger<AuthService>.Instance);

    [Fact]
    public async Task RegisterAsync_WithValidTenant_CreatesUserWithBcryptHash()
    {
        var tenant = _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);

        var result = await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", "Jane"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(tenant.Id, result.TenantId);
        Assert.Equal("jane@acme.test", result.Email);

        var user = context.Users.IgnoreQueryFilters().Single(u => u.Id == result.UserId);
        Assert.NotEqual("P@ssw0rd!", user.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("P@ssw0rd!", user.PasswordHash));
    }

    [Fact]
    public async Task RegisterAsync_WithUnknownTenant_ReturnsNull()
    {
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);

        var result = await service.RegisterAsync(
            new RegisterRequest("nope", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsNull()
    {
        _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);

        var first = await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);
        var duplicate = await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane2", "Other1!", null), CancellationToken.None);

        Assert.NotNull(first);
        Assert.Null(duplicate);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateUserName_ReturnsNull()
    {
        _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);

        var first = await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);
        var duplicate = await service.RegisterAsync(
            new RegisterRequest("acme", "other@acme.test", "jane", "Other1!", null), CancellationToken.None);

        Assert.NotNull(first);
        Assert.Null(duplicate);
    }

    [Fact]
    public async Task LoginAsync_WithCorrectCredentials_ReturnsUserAndUpdatesLastLogin()
    {
        _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);
        await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);

        var result = await service.LoginAsync(
            new LoginRequest("acme", "jane@acme.test", "P@ssw0rd!"), CancellationToken.None);

        Assert.NotNull(result);
        var user = context.Users.IgnoreQueryFilters().Single(u => u.Id == result.UserId);
        Assert.NotNull(user.LastLoginAt);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsNull()
    {
        _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);
        await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);

        var result = await service.LoginAsync(
            new LoginRequest("acme", "jane@acme.test", "wrong"), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ReturnsNull()
    {
        _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);
        var registered = await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);
        Assert.NotNull(registered);

        var user = context.Users.IgnoreQueryFilters().Single(u => u.Id == registered.UserId);
        user.IsActive = false;
        await context.SaveChangesAsync();

        var result = await service.LoginAsync(
            new LoginRequest("acme", "jane@acme.test", "P@ssw0rd!"), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsActiveGrants_ExcludingRevoked()
    {
        var tenant = _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);
        var registered = await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);
        Assert.NotNull(registered);

        _fixture.TenantProvider.TenantId = tenant.Id;
        var roles = context.Roles.ToDictionary(r => r.Name, r => r.Id);
        var orgId = Guid.NewGuid();
        context.UserAccesses.AddRange(
            new UserAccess
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                UserId = registered.UserId,
                RoleId = roles[RoleNames.OrgManager],
                ScopeType = AccessScopeType.Organization,
                ScopeId = orgId,
                GrantedAt = DateTimeOffset.UtcNow,
            },
            new UserAccess
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                UserId = registered.UserId,
                RoleId = roles[RoleNames.TenantAdmin],
                GrantedAt = DateTimeOffset.UtcNow,
                RevokedAt = DateTimeOffset.UtcNow,
            });
        await context.SaveChangesAsync();

        var result = await service.LoginAsync(
            new LoginRequest("acme", "jane@acme.test", "P@ssw0rd!"), CancellationToken.None);

        Assert.NotNull(result);
        var grant = Assert.Single(result.Grants);
        Assert.Equal(RoleNames.OrgManager, grant.RoleName);
        Assert.Equal(AccessScopeType.Organization, grant.ScopeType);
        Assert.Equal(orgId, grant.ScopeId);
    }
}

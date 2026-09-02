using KC.LMS.Storage.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace KC.LMS.Storage.Test;

[TestClass]
public sealed class AuthServiceTests
{
    private SqliteDbFixture _fixture = null!;

    [TestInitialize]
    public void Initialize() => _fixture = new SqliteDbFixture();

    [TestCleanup]
    public void Cleanup() => _fixture.Dispose();

    private AuthService CreateService(ApplicationDbContext context) =>
        new(context, NullLogger<AuthService>.Instance);

    [TestMethod]
    public async Task RegisterAsync_WithValidTenant_CreatesUserWithBcryptHash()
    {
        var tenant = _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);

        var result = await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", "Jane"), CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(tenant.Id, result.TenantId);
        Assert.AreEqual("jane@acme.test", result.Email);

        var user = context.Users.IgnoreQueryFilters().Single(u => u.Id == result.UserId);
        Assert.AreNotEqual("P@ssw0rd!", user.PasswordHash);
        Assert.IsTrue(BCrypt.Net.BCrypt.Verify("P@ssw0rd!", user.PasswordHash));
    }

    [TestMethod]
    public async Task RegisterAsync_WithUnknownTenant_ReturnsNull()
    {
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);

        var result = await service.RegisterAsync(
            new RegisterRequest("nope", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsNull()
    {
        _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);

        var first = await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);
        var duplicate = await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane2", "Other1!", null), CancellationToken.None);

        Assert.IsNotNull(first);
        Assert.IsNull(duplicate);
    }

    [TestMethod]
    public async Task LoginAsync_WithCorrectCredentials_ReturnsUserAndUpdatesLastLogin()
    {
        _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);
        await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);

        var result = await service.LoginAsync(
            new LoginRequest("acme", "jane@acme.test", "P@ssw0rd!"), CancellationToken.None);

        Assert.IsNotNull(result);
        var user = context.Users.IgnoreQueryFilters().Single(u => u.Id == result.UserId);
        Assert.IsNotNull(user.LastLoginAt);
    }

    [TestMethod]
    public async Task LoginAsync_WithWrongPassword_ReturnsNull()
    {
        _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);
        await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);

        var result = await service.LoginAsync(
            new LoginRequest("acme", "jane@acme.test", "wrong"), CancellationToken.None);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task LoginAsync_ExcludesRevokedGrants()
    {
        var tenant = _fixture.SeedTenant("acme");
        await using var context = _fixture.CreateContext();
        var service = CreateService(context);
        var registered = await service.RegisterAsync(
            new RegisterRequest("acme", "jane@acme.test", "jane", "P@ssw0rd!", null), CancellationToken.None);
        Assert.IsNotNull(registered);

        _fixture.TenantProvider.TenantId = tenant.Id;
        var roles = context.Roles.ToDictionary(r => r.Name, r => r.Id);
        context.UserAccesses.AddRange(
            new Entities.UserAccess
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                UserId = registered.UserId,
                RoleId = roles[Entities.RoleNames.Learner],
                GrantedAt = DateTimeOffset.UtcNow,
            },
            new Entities.UserAccess
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                UserId = registered.UserId,
                RoleId = roles[Entities.RoleNames.TenantAdmin],
                GrantedAt = DateTimeOffset.UtcNow,
                RevokedAt = DateTimeOffset.UtcNow,
            });
        await context.SaveChangesAsync();

        var result = await service.LoginAsync(
            new LoginRequest("acme", "jane@acme.test", "P@ssw0rd!"), CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.HasCount(1, result.Grants);
        Assert.AreEqual(Entities.RoleNames.Learner, result.Grants[0].RoleName);
    }
}

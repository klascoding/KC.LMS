using KC.LMS.Storage.Entities;

namespace KC.LMS.Storage.Test;

[TestClass]
public sealed class TenantQueryFilterTests
{
    private SqliteDbFixture _fixture = null!;

    [TestInitialize]
    public void Initialize() => _fixture = new SqliteDbFixture();

    [TestCleanup]
    public void Cleanup() => _fixture.Dispose();

    [TestMethod]
    public void Organizations_AreIsolatedPerTenant()
    {
        var tenantA = _fixture.SeedTenant("tenant-a");
        var tenantB = _fixture.SeedTenant("tenant-b");

        using (var seed = _fixture.CreateContext())
        {
            seed.Organizations.AddRange(
                new Organization { Id = Guid.NewGuid(), TenantId = tenantA.Id, Name = "Org A", CreatedAt = DateTimeOffset.UtcNow },
                new Organization { Id = Guid.NewGuid(), TenantId = tenantB.Id, Name = "Org B", CreatedAt = DateTimeOffset.UtcNow });
            seed.SaveChanges();
        }

        _fixture.TenantProvider.TenantId = tenantA.Id;
        using var context = _fixture.CreateContext();
        var visible = context.Organizations.ToList();

        Assert.HasCount(1, visible);
        Assert.AreEqual("Org A", visible[0].Name);
    }

    [TestMethod]
    public void Organizations_WithoutTenantContext_ReturnsNothing()
    {
        var tenant = _fixture.SeedTenant("tenant-a");
        using (var seed = _fixture.CreateContext())
        {
            seed.Organizations.Add(new Organization { Id = Guid.NewGuid(), TenantId = tenant.Id, Name = "Org A", CreatedAt = DateTimeOffset.UtcNow });
            seed.SaveChanges();
        }

        _fixture.TenantProvider.TenantId = null;
        using var context = _fixture.CreateContext();

        Assert.IsEmpty(context.Organizations.ToList());
    }

    [TestMethod]
    public void SystemRoles_AreVisibleToAnyTenant()
    {
        var tenant = _fixture.SeedTenant("tenant-a");
        _fixture.TenantProvider.TenantId = tenant.Id;
        using var context = _fixture.CreateContext();

        var roleNames = context.Roles.Select(r => r.Name).ToList();

        CollectionAssert.IsSubsetOf(
            new[] { RoleNames.TenantAdmin, RoleNames.OrgManager, RoleNames.Instructor, RoleNames.Learner },
            roleNames);
    }

    [TestMethod]
    public void TenantRoles_AreHiddenFromOtherTenants()
    {
        var tenantA = _fixture.SeedTenant("tenant-a");
        var tenantB = _fixture.SeedTenant("tenant-b");

        using (var seed = _fixture.CreateContext())
        {
            _fixture.TenantProvider.TenantId = tenantA.Id;
            seed.Roles.Add(new Role { Id = Guid.NewGuid(), TenantId = tenantA.Id, Name = "CustomRoleA" });
            seed.SaveChanges();
        }

        _fixture.TenantProvider.TenantId = tenantB.Id;
        using var context = _fixture.CreateContext();

        Assert.IsFalse(context.Roles.Any(r => r.Name == "CustomRoleA"));
    }
}

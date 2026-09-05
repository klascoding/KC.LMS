using KC.LMS.Storage;
using KC.LMS.Storage.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KC.LMS.Service.Test;

/// <summary>Mutable tenant provider so tests can switch the "current" tenant.</summary>
public sealed class TestTenantProvider : ITenantProvider
{
    public Guid? TenantId { get; set; }
}

/// <summary>Creates an ApplicationDbContext backed by a shared SQLite in-memory database.</summary>
public sealed class SqliteDbFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestTenantProvider TenantProvider { get; } = new();

    public SqliteDbFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;
        return new ApplicationDbContext(options, TenantProvider);
    }

    public Tenant SeedTenant(string slug)
    {
        using var context = CreateContext();
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = slug.ToUpperInvariant(),
            Slug = slug,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        context.Tenants.Add(tenant);
        context.SaveChanges();
        return tenant;
    }

    public void Dispose() => _connection.Dispose();
}

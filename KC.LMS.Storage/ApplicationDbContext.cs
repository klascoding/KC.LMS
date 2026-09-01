using KC.LMS.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace KC.LMS.Storage;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider)
    : DbContext(options)
{
    private readonly ITenantProvider _tenantProvider = tenantProvider;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserAccess> UserAccesses => Set<UserAccess>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(b =>
        {
            b.Property(t => t.Name).HasMaxLength(256).IsRequired();
            b.Property(t => t.Slug).HasMaxLength(128).IsRequired();
            b.HasIndex(t => t.Slug).IsUnique();
        });

        modelBuilder.Entity<Organization>(b =>
        {
            b.Property(o => o.Name).HasMaxLength(256).IsRequired();
            b.HasOne(o => o.Tenant).WithMany(t => t.Organizations).HasForeignKey(o => o.TenantId);
            b.HasQueryFilter(o => o.TenantId == _tenantProvider.TenantId);
        });

        modelBuilder.Entity<User>(b =>
        {
            b.Property(u => u.Email).HasMaxLength(320).IsRequired();
            b.Property(u => u.UserName).HasMaxLength(128).IsRequired();
            b.Property(u => u.PasswordHash).HasMaxLength(128).IsRequired();
            b.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
            b.HasIndex(u => new { u.TenantId, u.UserName }).IsUnique();
            b.HasOne(u => u.Tenant).WithMany(t => t.Users).HasForeignKey(u => u.TenantId);
            b.HasQueryFilter(u => u.TenantId == _tenantProvider.TenantId);
        });

        modelBuilder.Entity<UserAccess>(b =>
        {
            b.Property(a => a.Role).HasMaxLength(128).IsRequired();
            b.HasOne(a => a.User).WithMany(u => u.UserAccesses).HasForeignKey(a => a.UserId);
            b.HasIndex(a => new { a.TenantId, a.UserId, a.Role });
            b.HasQueryFilter(a => a.TenantId == _tenantProvider.TenantId);
        });

        modelBuilder.Entity<Department>(b =>
        {
            b.Property(d => d.Name).HasMaxLength(256).IsRequired();
            b.HasOne(d => d.Organization).WithMany(o => o.Departments).HasForeignKey(d => d.OrganizationId);
            b.HasOne(d => d.ParentDepartment).WithMany(d => d.ChildDepartments)
                .HasForeignKey(d => d.ParentDepartmentId).OnDelete(DeleteBehavior.Restrict);
            b.HasQueryFilter(d => d.TenantId == _tenantProvider.TenantId);
        });

        modelBuilder.Entity<Position>(b =>
        {
            b.Property(p => p.Title).HasMaxLength(256).IsRequired();
            b.HasQueryFilter(p => p.TenantId == _tenantProvider.TenantId);
        });

        modelBuilder.Entity<Employee>(b =>
        {
            b.Property(e => e.FirstName).HasMaxLength(128).IsRequired();
            b.Property(e => e.LastName).HasMaxLength(128).IsRequired();
            b.Property(e => e.EmployeeNumber).HasMaxLength(64);
            b.HasIndex(e => new { e.TenantId, e.EmployeeNumber }).IsUnique();
            b.HasOne(e => e.Organization).WithMany(o => o.Employees).HasForeignKey(e => e.OrganizationId);
            b.HasOne(e => e.Department).WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(e => e.Position).WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(e => e.User).WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId).OnDelete(DeleteBehavior.SetNull);
            b.HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId);
        });
    }
}

namespace KC.LMS.Storage.Entities;

/// <summary>A named role. <see cref="TenantId"/> is null for system roles shared across all tenants.</summary>
public class Role
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public Tenant? Tenant { get; set; }
    public ICollection<UserAccess> UserAccesses { get; set; } = [];
}

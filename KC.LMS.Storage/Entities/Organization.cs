namespace KC.LMS.Storage.Entities;

public class Organization : ITenantOwned
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }

    public Tenant? Tenant { get; set; }
    public ICollection<Department> Departments { get; set; } = [];
    public ICollection<Employee> Employees { get; set; } = [];
}

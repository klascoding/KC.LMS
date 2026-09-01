namespace KC.LMS.Storage.Entities;

public class Position : ITenantOwned
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Employee> Employees { get; set; } = [];
}

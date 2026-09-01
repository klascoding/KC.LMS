namespace KC.LMS.Storage.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<Organization> Organizations { get; set; } = [];
    public ICollection<User> Users { get; set; } = [];
}

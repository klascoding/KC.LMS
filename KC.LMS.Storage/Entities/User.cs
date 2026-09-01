namespace KC.LMS.Storage.Entities;

public class User : ITenantOwned
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }

    public Tenant? Tenant { get; set; }
    public Employee? Employee { get; set; }
    public ICollection<UserAccess> UserAccesses { get; set; } = [];
}

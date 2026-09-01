namespace KC.LMS.Storage.Entities;

public class Employee : ITenantOwned
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? EmployeeNumber { get; set; }
    public DateOnly? HireDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Organization? Organization { get; set; }
    public Department? Department { get; set; }
    public Position? Position { get; set; }
    public User? User { get; set; }
}

namespace KC.LMS.Storage.Entities;

/// <summary>Assigns a <see cref="Role"/> to a <see cref="User"/>, optionally scoped to an organization or department.</summary>
public class UserAccess : ITenantOwned
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    /// <summary>The hierarchy level this grant applies to. <see cref="AccessScopeType.Tenant"/> means tenant-wide (ScopeId null).</summary>
    public AccessScopeType ScopeType { get; set; } = AccessScopeType.Tenant;

    /// <summary>The Organization or Department id the grant is limited to; null for tenant-wide grants.</summary>
    public Guid? ScopeId { get; set; }

    public DateTimeOffset GrantedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }

    public User? User { get; set; }
    public Role? Role { get; set; }
}

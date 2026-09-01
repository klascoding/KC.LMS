namespace KC.LMS.Storage;

/// <summary>
/// Provides the tenant identity for the current execution scope (e.g., resolved from a JWT claim per request).
/// </summary>
public interface ITenantProvider
{
    /// <summary>The current tenant id, or <c>null</c> when no tenant context exists (e.g., registration).</summary>
    Guid? TenantId { get; }
}

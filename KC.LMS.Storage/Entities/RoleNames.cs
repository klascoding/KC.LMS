namespace KC.LMS.Storage.Entities;

/// <summary>Baseline role names. Seed these as system roles (TenantId = null) via migrations.</summary>
public static class RoleNames
{
    public const string TenantAdmin = "TenantAdmin";
    public const string OrgManager = "OrgManager";
    public const string Instructor = "Instructor";
    public const string Learner = "Learner";
}

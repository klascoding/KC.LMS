namespace KC.LMS.Storage.Entities;

public interface ITenantOwned
{
    Guid TenantId { get; set; }
}

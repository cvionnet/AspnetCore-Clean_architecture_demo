namespace Domain.Common;

/// <summary>
/// Class used to add tracking options on entities
/// </summary>
public class AuditableEntity
{
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}

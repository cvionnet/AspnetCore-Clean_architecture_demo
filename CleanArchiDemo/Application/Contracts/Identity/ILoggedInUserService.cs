namespace Application.Contracts.Identity;

/// <summary>
/// Initialized in "API project" Startup.cs
/// Can be used for "Domain project" AuditableEntity class (for CreatedBy or LastModifiedBy properties)
/// </summary>
public interface ILoggedInUserService
{
    public string UserId { get; }
}

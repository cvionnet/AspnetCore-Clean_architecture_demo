namespace Application.Features._DTOs
{
    /// <summary>
    /// Entity used to update an existing company
    /// </summary>
    public record UpdateCompanyDTO : AddCompanyDTO
    {
        public int CompanyID { get; set; }
    }
}

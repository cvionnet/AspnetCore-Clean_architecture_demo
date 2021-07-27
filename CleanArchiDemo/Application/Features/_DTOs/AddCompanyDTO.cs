using System;

namespace Application.Features._DTOs
{
    /// <summary>
    /// Entity used to add a new company
    /// </summary>
    public class AddCompanyDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string BillingAddress { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }
        public int CountryID { get; set; }
    }
}

using System;

namespace Application.Features._ViewModels
{
    public class CompanyVM
    {
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string BillingAddress { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }
        public string CountryName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

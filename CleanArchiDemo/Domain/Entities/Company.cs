using Domain.Common;

namespace Domain.Entities
{
    public class Company : AuditableEntity
    {
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string BillingAddress { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }
        public Country Country { get; set; }
    }
}

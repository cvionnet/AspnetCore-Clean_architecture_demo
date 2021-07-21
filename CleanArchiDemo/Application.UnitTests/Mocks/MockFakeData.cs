using Domain.Entities;
using System.Collections.Generic;

namespace Application.UnitTests.Mocks
{
    internal static class MockFakeData
    {
        internal static List<Country> FakeCountries = new List<Country>
            {
                new Country() { CountryID = 1, CountryName = "France" },
                new Country() { CountryID = 2, CountryName = "USA" }
            };

        internal static List<Company> FakeCompanies = new List<Company>
        {
            new Company
            {
                CompanyID=1,
                Name = "Company 1",
                Email = "company1@email.com",
                BillingAddress = "2 rue Centrale",
                Postcode = "75013",
                City = "Paris",
                Country = FakeCountries[0]
            },
            new Company
            {
                CompanyID=2,
                Name = "Company 2",
                Email = "company2@email.com",
                BillingAddress = "247 av du Professeur",
                Postcode = "69001",
                City = "Lyon",
                Country = FakeCountries[0]
            },
            new Company
            {
                CompanyID=3,
                Name = "Company 3",
                Email = "company3@email.com",
                BillingAddress = "65 North Elmwood Street",
                Postcode = "CA 91402",
                City = "Panorama City",
                Country = FakeCountries[1]
            }
        };
    }
}

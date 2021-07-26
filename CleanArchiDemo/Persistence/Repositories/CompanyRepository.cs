using Application.Contracts.Identity;
using Application.Contracts.Persistence;
using Dapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PersistenceDapper.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly string _connectionString;
        private readonly ILoggedInUserService _loggedInUserService;

        public CompanyRepository(IConfiguration configuration, ILoggedInUserService loggedInUserService)
        {
            _connectionString = configuration.GetConnectionString("ApplicationConnectionString");
            _loggedInUserService = loggedInUserService;
        }

        public async Task<IReadOnlyList<Company>> ListAllAsync()
        {
            var sql = @"SELECT CompanyID, Name, Email, BillingAddress, Postcode, City, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate, Country.CountryID, CountryName 
                            FROM Company INNER JOIN Country ON Company.CountryID = Country.CountryID;";

            var CompanyDict = new Dictionary<int, Company>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<Company, Country, Company>(
                    sql,
                    (company, country) => {
                        // check if the object has not already been added
                        if (!CompanyDict.TryGetValue(company.CompanyID, out var currentCompany))
                        {
                            currentCompany = company;
                            CompanyDict.Add(currentCompany.CompanyID, currentCompany);
                        }
                        currentCompany.Country = country;

                        return currentCompany;
                    },
                    //param: new { Idcompany = companyId },     // WHERE CompanyID = @Idcompany;";
                    splitOn: "CountryId");

                return result.ToList();

                //var result = await connection.QueryAsync<Company>(sql);
                //return result.ToList();
            }
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            var sql = @"SELECT CompanyID, Name, Email, BillingAddress, Postcode, City, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate, Country.CountryID, CountryName 
                            FROM Company INNER JOIN Country ON Company.CountryID = Country.CountryID
                            WHERE CompanyID = @Idcompany;";

            var CompanyDict = new Dictionary<int, Company>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<Company, Country, Company>(
                    sql,
                    (company, country) => {
                        // check if the object has not already been added
                        if (!CompanyDict.TryGetValue(company.CompanyID, out var currentCompany))
                        {
                            currentCompany = company;
                            CompanyDict.Add(currentCompany.CompanyID, currentCompany);
                        }
                        currentCompany.Country = country;

                        return currentCompany;
                    },
                    param: new { Idcompany = id },
                    splitOn: "CountryId");

                return result.FirstOrDefault();
            }
        }

        public async Task<Company> AddAsync(Company entity)
        {
            var sql = @"INSERT INTO Users (Email, Login, Name, FirstName, Password)
                                VALUES(@Email, @Login, @Name, @FirstName, @Password);";

            //TODO : prendre en compte les champs d'audit (AuditableEntity)
            // CreatedDate = DateTime.Now  /  CreatedBy = _loggedInUserService .UserId          LastModifiedDate = DateTime.Now;  /  LastModifiedBy = _loggedInUserService.UserId;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // ExecuteAsync returns the number of lines affected
                //var affectedRows = await connection.ExecuteAsync(sql, entity);
                //return affectedRows;

                // QueryAsync<>  to get the id created during the SQL session
                //sql = sql + "SELECT CAST(SCOPE_IDENTITY() as int);";
                //var lastId = await connection.QueryAsync<int>(sql, entity);
                //return lastId.Single();

                // QueryAsync<>  to get the id created during the SQL session
                sql = sql + "SELECT CAST(SCOPE_IDENTITY() as int);";
                var lastId = await connection.QueryAsync<int>(sql, entity);
                return await GetByIdAsync(lastId.Single());
            }
        }

        public async Task<int> DeleteAsync(Company entity)
        {
            var sql = "DELETE FROM Users WHERE UserId = @Id;";
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
//                var affectedRows = await connection.ExecuteAsync(sql, new { Id = entity.UserId });
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = 1 });
                return affectedRows;
            }
        }

        public async Task<int> UpdateAsync(Company entity)
        {
            var sql = @"UPDATE Users SET Email = @Email, Login = @Login, Name = @Name, FirstName = @FirstName 
                                WHERE UserId = @UserId;";

            //TODO : prendre en compte les champs d'audit (AuditableEntity)
            // CreatedDate = DateTime.Now  /  CreatedBy = _loggedInUserService .UserId          LastModifiedDate = DateTime.Now;  /  LastModifiedBy = _loggedInUserService.UserId;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var affectedRows = await connection.ExecuteAsync(sql, entity);
                return affectedRows;
            }
        }

        public Task<bool> ResetDBToDemo()
        {
            throw new NotImplementedException();
        }
    }
}

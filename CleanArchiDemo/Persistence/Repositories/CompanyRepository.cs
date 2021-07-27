using Application.Contracts.Identity;
using Application.Contracts.Persistence;
using Dapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SerilogTimings;
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
        private readonly ILogger<CompanyRepository> _logger;

        public CompanyRepository(IConfiguration configuration, ILoggedInUserService loggedInUserService, ILogger<CompanyRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("ApplicationConnectionString");
            _loggedInUserService = loggedInUserService;
            _logger = logger;
        }

        public async Task<IReadOnlyList<Company>> ListAllAsync()
        {
            var sql = @"SELECT CompanyID, Name, Email, BillingAddress, Postcode, City, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate, Country.CountryID, CountryName 
                            FROM Company INNER JOIN Country ON Company.CountryID = Country.CountryID;";

            var CompanyDict = new Dictionary<int, Company>();

            using (Operation.Time($"[{GetType().Name}] Timing (DB) - Get all companies"))
            {
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
                        splitOn: "CountryId");

                    _logger.LogInformation($"USER:{_loggedInUserService.UserId} - Get all companies from database");
                    return result.ToList();
                }
            }
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            var sql = @"SELECT CompanyID, Name, Email, BillingAddress, Postcode, City, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate, Country.CountryID, CountryName 
                            FROM Company INNER JOIN Country ON Company.CountryID = Country.CountryID
                            WHERE CompanyID = @Idcompany;";

            var CompanyDict = new Dictionary<int, Company>();

            using (Operation.Time($"[{GetType().Name}] Timing (DB) - Get company {id}"))
            {
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

                    _logger.LogInformation($"USER:{_loggedInUserService.UserId} - Get company {id} from database");
                    return result.FirstOrDefault();
                }
            }
        }

        public async Task<Company> AddAsync(Company entity)
        {
            var sql = @"INSERT INTO Company (Name, Email, BillingAddress, Postcode, City, CountryID, CreatedBy, CreatedDate)
                                VALUES(@Name, @Email, @BillingAddress, @Postcode, @City, @CountryID, @CreatedBy, @CreatedDate);";

            using (Operation.Time($"[{GetType().Name}] Timing (DB) - Add new company (name:{entity.Name})"))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // OPTION 1 : ExecuteAsync returns the number of lines affected
                    //var affectedRows = await connection.ExecuteAsync(sql, entity);
                    //return affectedRows;

                    // OPTION 2 : QueryAsync<>  to get the id created during the SQL session
                    //sql = sql + "SELECT CAST(SCOPE_IDENTITY() as int);";
                    //var lastId = await connection.QueryAsync<int>(sql, entity);
                    //return lastId.Single();

                    // OPTION 3 : QueryAsync<>  to get the id created during the SQL session + get the entity object created
                    sql = sql + "SELECT CAST(SCOPE_IDENTITY() as int);";

                    // case 1 : if the entity does not have nested object, you can use 'entity' as parameter  (⚠️ CreatedBy and CreatedDate have no values)
                    //var lastId = await connection.QueryAsync<int>(sql, entity);

                    // case 2 : if the entity have nested object, you must specify each parameter in the query
                    var lastId = await connection.QueryAsync<int>(sql, new
                    {
                        Name = entity.Name,
                        Email = entity.Email,
                        BillingAddress = entity.BillingAddress,
                        Postcode = entity.Postcode,
                        City = entity.City,
                        CountryID = entity.Country.CountryID,       // nested object
                        CreatedBy = _loggedInUserService.UserId,
                        CreatedDate = DateTime.Now
                    });

                    return await GetByIdAsync(lastId.Single());
                }
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

        public async Task<int> DeleteAsync(Company entity)
        {
            var sql = "DELETE FROM Company WHERE CompanyId = @Id;";

            using (Operation.Time($"[{GetType().Name}] Timing (DB) - Delete company {entity.CompanyID}"))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var affectedRows = await connection.ExecuteAsync(sql, new { Id = entity.CompanyID });

                    _logger.LogInformation($"USER:{_loggedInUserService.UserId} - Delete company from database (id:{entity.CompanyID}, name:{entity.Name}, mail:{entity.Email}");
                    return affectedRows;
                }
            }
        }

        /// <summary>
        /// Check if the name already exists in the database
        /// </summary>
        /// <param name="name">The value to check</param>
        /// <returns>True if the value is not present</returns>
        public async Task<bool> isNameUnique(string name)
        {
            var sql = "SELECT COUNT(Name) FROM Company WHERE Name = @Name;";

            using (Operation.Time($"[{GetType().Name}] Timing (DB) - Check company name:{name}"))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<int>(sql, new { Name = name });

                    _logger.LogInformation($"USER:{_loggedInUserService.UserId} - Check for name uniqueness (name:{name})");
                    return !result.Any(x => x >= 1);
                }
            }
        }



        public Task<bool> ResetDBToDemo()
        {
            throw new NotImplementedException();
        }
    }
}

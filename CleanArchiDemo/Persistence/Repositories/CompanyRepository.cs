using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SerilogTimings;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PersistenceDapper.Repositories;

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

        using (Operation.Time($"[{GetType().Name}][TIMING_DB]Get all companies"))
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

                _logger.LogInformation($"[USER:{_loggedInUserService.UserId}]Get all companies from DB");
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

        using (Operation.Time($"[{GetType().Name}][TIMING_DB]Get company {id}"))
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

                _logger.LogInformation($"[USER:{_loggedInUserService.UserId}]Get company {id} from DB");
                return result.FirstOrDefault();
            }
        }
    }

    public async Task<Company> AddAsync(Company entity)
    {
        var sql = @"INSERT INTO Company (Name, Email, BillingAddress, Postcode, City, CountryID, CreatedBy, CreatedDate)
                                VALUES(@Name, @Email, @BillingAddress, @Postcode, @City, @CountryID, @CreatedBy, @CreatedDate);";

        using (Operation.Time($"[{GetType().Name}][TIMING_DB]Add new company (name:{entity.Name})"))
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

                _logger.LogInformation($"[USER:{_loggedInUserService.UserId}]Add company in DB (id:{entity.CompanyID}, name:{entity.Name})");
                return await GetByIdAsync(lastId.Single());
            }
        }
    }

    public async Task<Company> UpdateAsync(Company entity)
    {
        var sql = @"UPDATE Company SET Name = @Name, Email = @Email, BillingAddress = @BillingAddress, Postcode = @Postcode,
                                City = @City, CountryID = @CountryID, LastModifiedBy = @LastModifiedBy, LastModifiedDate = @LastModifiedDate
                                WHERE CompanyID = @CompanyID;";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            // case 1 : if the entity does not have nested object, you can use 'entity' as parameter  (⚠️ LastModifiedBy and LastModifiedDate have no values)
            //var affectedRows = await connection.ExecuteAsync(sql, entity);

            // case 2 : if the entity have nested object, you must specify each parameter in the query
            await connection.ExecuteAsync(sql, new
            {
                CompanyID = entity.CompanyID,
                Name = entity.Name,
                Email = entity.Email,
                BillingAddress = entity.BillingAddress,
                Postcode = entity.Postcode,
                City = entity.City,
                CountryID = entity.Country.CountryID,       // nested object
                LastModifiedBy = _loggedInUserService.UserId,
                LastModifiedDate = DateTime.Now
            });

            _logger.LogInformation($"[USER:{_loggedInUserService.UserId}]Update company from DB (id:{entity.CompanyID})");
            return await GetByIdAsync(entity.CompanyID);
        }
    }

    public async Task<int> DeleteAsync(Company entity)
    {
        var sql = "DELETE FROM Company WHERE CompanyId = @Id;";

        using (Operation.Time($"[{GetType().Name}][TIMING_DB]Delete company {entity.CompanyID}"))
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = entity.CompanyID });

                _logger.LogInformation($"[USER:{_loggedInUserService.UserId}]Delete company from DB (id:{entity.CompanyID}, name:{entity.Name}, mail:{entity.Email})");
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

        using (Operation.Time($"[{GetType().Name}][TIMING_DB]Check company name:{name}"))
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<int>(sql, new { Name = name });

                _logger.LogInformation($"[USER:{_loggedInUserService.UserId}]Check for name uniqueness (name:{name})");
                return !result.Any(x => x >= 1);
            }
        }
    }

    /// <summary>
    /// Check if the country ID exists in the database
    /// </summary>
    /// <param name="id">The id to search</param>
    /// <returns>True if the value is present</returns>
    public async Task<bool> isCountryIdExists(int id)
    {
        var sql = "SELECT COUNT(CountryID) FROM Country WHERE CountryID = @Id;";

        using (Operation.Time($"[{GetType().Name}][TIMING_DB]Check country ID presence:{id}"))
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<int>(sql, new { Id = id });

                _logger.LogInformation($"[USER:{_loggedInUserService.UserId}]Check for country ID presence (id:{id})");
                return !result.Any(x => x >= 1);
            }
        }
    }

    public Task<bool> ResetDBToDemo()
    {
        throw new NotImplementedException();
    }
}

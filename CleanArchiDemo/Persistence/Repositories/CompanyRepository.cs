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
        private readonly string connectionString;
        public CompanyRepository(IConfiguration configuration) => connectionString = configuration.GetConnectionString("FollowUpConnectionString");

        //public Task<User> GetByIdAsync(Guid id)
        public async Task<Company> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Users WHERE UserId = @Id;";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<Company>(sql, new { Id = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Company>> ListAllAsync()
        {
            var sql = "SELECT * FROM Users";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<Company>(sql);
                return result;
            }
        }

        public async Task<int> AddAsync(Company entity)
        {
            var sql = @"INSERT INTO Users (Email, Login, Name, FirstName, Password)
                                VALUES(@Email, @Login, @Name, @FirstName, @Password);";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // ExecuteAsync returns the number of lines affected
                //var affectedRows = await connection.ExecuteAsync(sql, entity);
                //return affectedRows;

                // QueryAsync<>  to get the id created during the SQL session
                sql = sql + "SELECT CAST(SCOPE_IDENTITY() as int);";
                var lastId = await connection.QueryAsync<int>(sql, entity);
                return lastId.Single();
            }
        }

        public async Task<int> DeleteAsync(Company entity)
        {
            var sql = "DELETE FROM Users WHERE UserId = @Id;";
            using (var connection = new SqlConnection(connectionString))
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
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var affectedRows = await connection.ExecuteAsync(sql, entity);
                return affectedRows;
            }
        }

        /// <summary>
        /// Check if the email already exists in the database
        /// </summary>
        /// <param name="email">The value to check</param>
        /// <returns>True if the value is not present</returns>
        public async Task<bool> isEmailUnique(string email)
        {
            var sql = "SELECT COUNT(Email) FROM Users WHERE Email = @Mail;";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<int>(sql, new { Mail = email });

                return !result.Any(x => x >= 1);
            }
        }

        /// <summary>
        /// Check if user / password exists in the database
        /// </summary>
        /// <param name="login">The login of the user</param>
        /// <param name="password">The encrypted password of the user</param>
        /// <returns>True if the user exists and password is correct</returns>
        public async Task<bool> ConnectUser(string login, string password)
        {
            var sql = "SELECT COUNT(UserId) FROM Users WHERE Login = @Login AND Password = @Pwd;";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<int>(sql, new { Login = login, Pwd = password });

                return result.Any(x => x >= 1);
            }
        }







        public Task<bool> ResetDBToDemo()
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<Company>> IAsyncRepository<Company>.ListAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}

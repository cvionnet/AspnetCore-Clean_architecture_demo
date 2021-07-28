using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Contracts.Persistence
{
    public interface ICompanyRepository : IAsyncRepository<Company>
    {
        // Used for Validator
        Task<bool> isNameUnique(string name);
        Task<bool> isCountryIdExists(int id);

        // Force a reset of the database (demonstration version)
        Task<bool> ResetDBToDemo();
    }
}

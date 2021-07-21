using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Contracts.Persistence
{
    public interface ICompanyRepository : IAsyncRepository<Company>
    {
        // Force a reset of the database (demonstration version)
        Task<bool> ResetDBToDemo();
    }
}

using Application.Contracts.Persistence;

namespace PersistenceDapper.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(ICompanyRepository companyRepository)
        {
            Companies = companyRepository;
        }

        public ICompanyRepository Companies { get; }
    }
}
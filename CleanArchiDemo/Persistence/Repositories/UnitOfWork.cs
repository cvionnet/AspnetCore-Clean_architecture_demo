namespace PersistenceDapper.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public ICompanyRepository Companies { get; }

    public UnitOfWork(ICompanyRepository companyRepository)
    {
        Companies = companyRepository;
    }
}
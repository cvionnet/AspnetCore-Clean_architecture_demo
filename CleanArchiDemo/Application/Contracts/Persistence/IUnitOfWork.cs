namespace Application.Contracts.Persistence
{
    public interface IUnitOfWork
    {
        ICompanyRepository Companies { get; }
    }
}
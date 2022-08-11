namespace Application.Contracts.Persistence;

/// <summary>
/// Contains all generic CRUD methods
/// </summary>
public interface IAsyncRepository<T> where T : class
{
    //Task<T> GetByIdAsync(Guid id);
    Task<T> GetByIdAsync(int id);
    //Task<IEnumerable<T>> ListAllAsync();
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<int> DeleteAsync(T entity);    // return the number of lines deleted
}

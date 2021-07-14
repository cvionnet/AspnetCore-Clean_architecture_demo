using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Contracts.Persistence
{
    /// <summary>
    /// Contains all generic CRUD methods
    /// </summary>
    public interface IAsyncRepository<T> where T : class
    {
        //Task<T> GetByIdAsync(Guid id);
        Task<T> GetByIdAsync(int id);
        //Task<IEnumerable<T>> ListAllAsync();
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<int> AddAsync(T entity);       // return the ID of the line inserted
        Task<int> UpdateAsync(T entity);    // return the number of lines updated
        Task<int> DeleteAsync(T entity);    // return the number of lines deleted
    }
}


using System.Linq.Expressions;

namespace Play.Common
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T entity);
        Task<IReadOnlyCollection<T>> GetAllAsync();

        // overloaded GetAllAsync function, takes a query expression MongoDB can read
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<T> GetAsync(Guid id);

        //use filter to retrieve any entity that matches that filter
        // Func<T, bool> : takes a parameter T and returns a boolean
        // Expression<T> : represents a function or lamda expression
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(T entity);
    }
}
using System.Linq.Expressions;
using workshop.wwwapi.Models;

namespace workshop.wwwapi.Repository
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);

        Task<T> Add(T entity);

        Task<IEnumerable<T>> GetWithIncludes(params Expression<Func<T, object>>[] includes);

    }
}

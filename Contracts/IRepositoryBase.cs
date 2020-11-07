using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Leave_management.Contracts
{
	public interface IRepositoryBase<T> where T: class
	{
        Task <ICollection<T>> FindAll();
        Task <T> FindById(int id);
        Task <bool> IsExists(int id);
        Task <bool> Create(T entity);
        Task <bool> Update(T entity);
        Task <bool> Delete(T entity);
        Task <bool> Save();
    }
    public interface IGenericRepository<T> where T : class
    {
        Task<IList<T>> FindAll
            (
             Expression<Func<T,bool>> expression = null,
             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
             List<string> includes = null
            );

        Task<T> Find
            (
             Expression<Func<T, bool>> expression,
             List<string> includes = null
            );

		Task<bool> IsExists(Expression<Func<T, bool>> expression = null);
		Task Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}

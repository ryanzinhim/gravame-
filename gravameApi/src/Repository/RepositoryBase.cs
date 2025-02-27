using Common.Database;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using gravameApi.src.Interface;

namespace gravameApi.src.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected Context RepositoryContext { get; set; }
        public RepositoryBase(Context repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public IQueryable<T> FindAll() => RepositoryContext.Set<T>().AsNoTracking();
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
            RepositoryContext.Set<T>().Where(expression).AsNoTracking();
        public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
        public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);
        public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);
    }
}

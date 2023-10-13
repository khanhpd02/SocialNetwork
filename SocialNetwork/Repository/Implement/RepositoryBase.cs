using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity;
using System.Linq.Expressions;

namespace SocialNetwork.Repository.Implement
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class, IEntity
    {

        protected readonly SocialNetworkContext context;

        protected RepositoryBase(SocialNetworkContext context)
        {
            this.context = context;
        }

        public List<T> FindAll(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>().Where(x => x.IsDeleted == false);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.AsNoTracking().ToList();
        }

        public List<T> FindByCondition(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>().Where(expression);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.AsNoTracking().ToList();
        }

        public List<T> FindByConditionWithTracking(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>().Where(expression);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.ToList();
        }

        public T FindById(Guid? Id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>().Where(x => x.Id == Id & x.IsDeleted == false);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.AsNoTracking().FirstOrDefault()!;
        }

        public void Create(T entity)
        {
            entity.IsDeleted = false;
            entity.CreateDate = DateTime.Now;
            context.Set<T>().Add(entity);
        }
        public void CreateIsTemp(T entity)
        {
            entity.IsDeleted = true;
            entity.CreateDate = DateTime.Now;
            context.Set<T>().Add(entity);
        }
        public void Update(T entity) => context.Set<T>().Update(entity);

        public void Delete(T entity) => context.Set<T>().Remove(entity);

        public void Save() => context.SaveChanges();

        public void Attach(Object entity) => context.Attach(entity);

        public IQueryable<T> QueryAll(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.AsNoTracking();
        }
    }
}

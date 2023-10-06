using System.Linq.Expressions;

namespace SocialNetwork.Repository
{
    public interface IRepositoryBase<T>
    {
        List<T> FindAll(params Expression<Func<T, object>>[] includes);

        List<T> FindByCondition(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);

        List<T> FindByConditionWithTracking(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);

        T FindById(Guid Id, params Expression<Func<T, object>>[] includes);

        void Create(T entity);

        void Update(T entity);

        void Delete(T entity);

        void Save();

        void Attach(Object entity);
        IQueryable<T> QueryAll(params Expression<Func<T, object>>[] includes);
    }
}

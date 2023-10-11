using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity;
using System.Linq.Expressions;

namespace SocialNetwork.Repository.Implement
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly SocialNetworkContext context;

        public RefreshTokenRepository(SocialNetworkContext context)
        {
            this.context = context;
        }

        public List<RefreshToken> FindAll(params Expression<Func<RefreshToken, object>>[] includes)
        {
            IQueryable<RefreshToken> query = context.Set<RefreshToken>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.AsNoTracking().ToList();
        }

        public List<RefreshToken> FindByCondition(Expression<Func<RefreshToken, bool>> expression,
            params Expression<Func<RefreshToken, object>>[] includes)
        {
            IQueryable<RefreshToken> query = context.Set<RefreshToken>().Where(expression);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.AsNoTracking().ToList();
        }

        public void Create(RefreshToken entity) => context.Set<RefreshToken>().Add(entity);

        public void Update(RefreshToken entity) => context.Set<RefreshToken>().Update(entity);

        public void Delete(RefreshToken entity) => context.Set<RefreshToken>().Remove(entity);

        public void Save() => context.SaveChanges();

        public RefreshToken FindById(Guid Id, params Expression<Func<RefreshToken, object>>[] includes)
        {
            IQueryable<RefreshToken> query = context.Set<RefreshToken>().Where(x => x.Id == Id);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.AsNoTracking().FirstOrDefault()!;
        }
    }
}

using SocialNetwork.Entity;
using System.Linq.Expressions;

namespace SocialNetwork.Repository
{
    public interface IRefreshTokenRepository
    {
        List<RefreshToken> FindAll(params Expression<Func<RefreshToken, object>>[] includes);

        List<RefreshToken> FindByCondition(Expression<Func<RefreshToken, bool>> expression, params Expression<Func<RefreshToken, object>>[] includes);

        void Create(RefreshToken entity);

        void Update(RefreshToken entity);

        void Delete(RefreshToken entity);

        void Save();

        RefreshToken FindById(Guid Id, params Expression<Func<RefreshToken, object>>[] includes);
    }
}

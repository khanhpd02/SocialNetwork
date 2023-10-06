using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class LikeRepository : RepositoryBase<Like>, ILikeRepository
    {
        public LikeRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

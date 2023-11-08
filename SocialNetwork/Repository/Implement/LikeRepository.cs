using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class LikeRepository : RepositoryBase<Like>, ILikeRepository
    {
        public LikeRepository(SocialNetworkContext context, IGeneralService generalService) : base(context, generalService)
        {
        }
    }
}

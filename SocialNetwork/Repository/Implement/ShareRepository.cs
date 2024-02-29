using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class ShareRepository : RepositoryBase<Share>, IShareRepository
    {
        public ShareRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

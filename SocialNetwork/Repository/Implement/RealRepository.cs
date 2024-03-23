
using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class RealRepository : RepositoryBase<Real>, IRealRepository
    {
        public RealRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

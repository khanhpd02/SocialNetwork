using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

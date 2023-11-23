using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class UserRoleRepository : RepositoryBase<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

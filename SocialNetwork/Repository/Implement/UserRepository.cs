using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

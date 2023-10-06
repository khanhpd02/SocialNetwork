using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

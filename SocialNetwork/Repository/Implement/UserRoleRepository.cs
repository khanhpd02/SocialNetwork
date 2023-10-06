using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class UserRoleRepository : RepositoryBase<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

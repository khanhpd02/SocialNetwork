using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class GroupRepository : RepositoryBase<Group>, IGroupRepository
    {
        public GroupRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

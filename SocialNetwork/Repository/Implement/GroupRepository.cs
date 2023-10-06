using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class GroupRepository : RepositoryBase<Group>, IGroupRepository
    {
        public GroupRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

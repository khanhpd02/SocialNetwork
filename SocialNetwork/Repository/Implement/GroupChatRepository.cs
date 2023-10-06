using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class GroupChatRepository : RepositoryBase<GroupChat>, IGroupChatRepository
    {
        public GroupChatRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class GroupChatRepository : RepositoryBase<GroupChat>, IGroupChatRepository
    {
        public GroupChatRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

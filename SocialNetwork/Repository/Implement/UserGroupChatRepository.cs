
using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class UserGroupChatRepository : RepositoryBase<UserGroupChat>, IUserGroupChatRepository
    {
        public UserGroupChatRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

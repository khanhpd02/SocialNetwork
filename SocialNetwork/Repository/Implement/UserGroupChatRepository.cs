
using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class UserGroupChatRepository : RepositoryBase<UserGroupChat>, IUserGroupChatRepository
    {
        public UserGroupChatRepository(SocialNetworkContext context, IGeneralService generalService) : base(context, generalService)
        {
        }
    }
}

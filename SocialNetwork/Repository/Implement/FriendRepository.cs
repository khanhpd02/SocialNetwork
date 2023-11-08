using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class FriendRepository : RepositoryBase<Friend>, IFriendRepository
    {
        public FriendRepository(SocialNetworkContext context, IGeneralService generalService) : base(context, generalService)
        {
        }
    }
}

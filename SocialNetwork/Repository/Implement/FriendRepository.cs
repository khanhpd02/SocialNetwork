using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class FriendRepository : RepositoryBase<Friend>, IFriendRepository
    {
        public FriendRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

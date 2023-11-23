using SocialNetwork.Entity;
using SocialNetwork.Service;

namespace SocialNetwork.Repository.Implement
{
    public class NotifyRepository : RepositoryBase<Notify>, INotifyRepository
    {
        public NotifyRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }

}



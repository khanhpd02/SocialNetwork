
using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class AudioRepository : RepositoryBase<Audio>, IAudioRepository
    {
        public AudioRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

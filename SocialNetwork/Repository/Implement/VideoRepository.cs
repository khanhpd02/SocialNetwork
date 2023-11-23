
using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class VideoRepository : RepositoryBase<Video>, IVideoRepository
    {
        public VideoRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

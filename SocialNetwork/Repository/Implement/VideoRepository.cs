
using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class VideoRepository : RepositoryBase<Video>, IVideoRepository
    {
        public VideoRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

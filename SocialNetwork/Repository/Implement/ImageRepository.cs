using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class ImageRepository : RepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

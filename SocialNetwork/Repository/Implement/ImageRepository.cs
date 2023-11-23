using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class ImageRepository : RepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

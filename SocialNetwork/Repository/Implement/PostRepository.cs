using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

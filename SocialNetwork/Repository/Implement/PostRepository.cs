using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

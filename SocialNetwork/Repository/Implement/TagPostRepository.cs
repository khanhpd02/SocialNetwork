using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class TagPostRepository : RepositoryBase<TagPost>, ITagPostRepository
    {
        public TagPostRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

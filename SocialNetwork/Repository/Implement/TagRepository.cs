
using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Repository
{

    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(SocialNetworkContext context) : base(context)
        {
        }
    }
}

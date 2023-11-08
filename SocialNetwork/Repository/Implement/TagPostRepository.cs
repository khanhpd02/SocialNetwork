using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class TagPostRepository : RepositoryBase<TagPost>, ITagPostRepository
    {
        public TagPostRepository(SocialNetworkContext context, IGeneralService generalService) : base(context, generalService)
        {
        }
    }
}

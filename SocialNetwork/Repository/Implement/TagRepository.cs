
using SocialNetwork.Entity;
using SocialNetwork.Repository.Implement;
using SocialNetwork.Service;

namespace SocialNetwork.Repository
{

    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(SocialNetworkContext context, IUserService userService) : base(context, userService)
        {
        }
    }
}

using SocialNetwork.Entity;
using SocialNetwork.Service;

namespace SocialNetwork.Repository.Implement
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(SocialNetworkContext context, IGeneralService generalService) : base(context, generalService)
        {
        }
    }

}



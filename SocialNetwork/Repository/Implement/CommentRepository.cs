using SocialNetwork.Entity;

namespace SocialNetwork.Repository.Implement
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(SocialNetworkContext context) : base(context)
        {
        }
    }

}



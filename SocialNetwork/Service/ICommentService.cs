using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;

namespace SocialNetwork.Service
{
    public interface ICommentService
    {
        AppResponse create(CommentDTO commentDTO, Guid userId);
        AppResponse update(CommentDTO commentDTO, Guid userID);
        List<CommentDTO> getAllOnPost(Guid postId);
        List<CommentDTO> getallofUser(Guid userId);
        List<CommentDTO> getallofUseronPost(Guid postId, Guid userId);
        AppResponse deleteOfUndo(Guid commentId, Guid userId);
    }
}

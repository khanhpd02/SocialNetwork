using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;

namespace SocialNetwork.Service
{
    public interface ICommentService
    {
        AppResponse create(CommentDTO commentDTO);
        AppResponse update(CommentDTO commentDTO);
        List<CommentDTO> getAllOnPost(Guid postId);
        List<CommentDTO> getallofUser(Guid userId);
        List<CommentDTO> getallofUseronPost(Guid postId);
        AppResponse deleteOfUndo(Guid commentId);
    }
}

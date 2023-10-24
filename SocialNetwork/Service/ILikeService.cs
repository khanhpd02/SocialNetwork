using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;
using SocialNetwork.Entity;

namespace SocialNetwork.Service
{
    public interface ILikeService
    {
        AppResponse LikeAndUnlike(Guid postId,Guid userid);
        AppResponse updateLike(LikeDTO likedto,Guid userId);
        AppResponse deleteLike(Guid idLike);
        List<LikeDTO> getallByPostId(Guid postId);
        List<LikeDTO> getallByUserID(Guid userId);

    }
}

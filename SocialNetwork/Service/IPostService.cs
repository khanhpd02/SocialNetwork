using SocialNetwork.DTO;
using SocialNetwork.DTO.UpdateDTO;

namespace SocialNetwork.Service
{
    public interface IPostService
    {
        List<PostDTO> GetPostsAndShareByUserId(Guid userId);
        List<string> UploadFilesToCloudinary(List<IFormFile> files);
        PostDTO Create(PostDTO dto);
        Task<PostDTO> Update(PostUpdateDTO dto);
        ShareDTO SharePost(ShareDTO sharePostDTO);
        PostDTO GetById(Guid id);
        void Delete(Guid id);
        List<PostDTO> GetAllPostsAndShare(int numberOfPosts);
    }
}

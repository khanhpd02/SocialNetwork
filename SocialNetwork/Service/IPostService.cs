using SocialNetwork.DTO;

namespace SocialNetwork.Service
{
    public interface IPostService
    {
        List<PostDTO> GetPostByUserId(Guid id);
        List<string> UploadFilesToCloudinary(List<IFormFile> files);
        PostDTO Create(PostDTO dto);
        PostDTO Update(PostDTO dto);
        PostDTO GetById(Guid id);
        void Delete(Guid id);
        List<PostDTO> GetAll();
    }
}

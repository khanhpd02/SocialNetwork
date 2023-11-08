using SocialNetwork.DTO;
using SocialNetwork.DTO.Cloudinary;

namespace SocialNetwork.Service
{
    public interface IPostService
    {
        string UploadFileToCloudinary(FileUploadDTO fileUploadDTO);
        PostDTO Create(PostDTO dto);
        PostDTO Update(PostDTO dto);
        PostDTO GetById(Guid id);
        void Delete(Guid id);
        List<PostDTO> GetAll();
    }
}

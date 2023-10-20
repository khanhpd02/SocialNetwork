using SocialNetwork.DTO;

namespace SocialNetwork.Service
{
    public interface IPostService
    {
        PostDTO Create(PostDTO dto, string userEmail);
        PostDTO GetById(Guid id);
        void Delete(Guid id);
        List<PostDTO> GetAll();
    }
}

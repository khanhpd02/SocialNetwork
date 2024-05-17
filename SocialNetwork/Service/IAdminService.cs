using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;

namespace SocialNetwork.Service
{
    public interface IAdminService
    {
        List<PostDTO> GetAllPosts();
        AppResponse DeletePostById(Guid postId);
        UserDTO GetUserById(Guid userId);
        UserDTO GetUserByEmail(string Email);
        List<InforDTO> GetAllUser();
        AppResponse CreateAdmin();
        AppResponse DeleteAllUser();
        AppResponse BanUserById(Guid userId);
    }
}

using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;

namespace SocialNetwork.Service
{
    public interface IAdminService
    {
        UserDTO GetUserById(Guid userId);
        UserDTO GetUserByEmail(string Email);
        List<UserDTO> GetAllUser();
        AppResponse CreateAdmin();
        AppResponse DeleteAllUser();

    }
}

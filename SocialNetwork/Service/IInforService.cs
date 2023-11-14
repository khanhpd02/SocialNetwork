using SocialNetwork.DTO;
using SocialNetwork.DTO.Cloudinary;
using SocialNetwork.DTO.Response;

namespace SocialNetwork.Service
{
    public interface IInforService
    {
        AppResponse createInfo(InforDTO inforDTO, Guid userId);
        AppResponse updateInfo(InforDTO inforDTO, Guid userId);
        AppResponse deleteInfo(Guid id, Guid userId);

    }
}

using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;

namespace SocialNetwork.Service
{
    public interface IInforService
    {
        InforDTO GetInforByUserId(Guid id);
        AppResponse createInfo(InforDTO inforDTO, Guid userId);
        AppResponse updateInfo(InforDTO inforDTO, Guid userId);
        AppResponse deleteInfo(Guid id, Guid userId);

    }
}

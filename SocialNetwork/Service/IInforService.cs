using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;

namespace SocialNetwork.Service
{
    public interface IInforService
    {
        Task<InforDTO> GetMyInfor();
        InforDTO GetInforByUserId(Guid id);
        AppResponse createInfo(InforDTO inforDTO, Guid userId);
        AppResponse updateInfo(InforDTO inforDTO, Guid userId);
        AppResponse deleteInfo(Guid id, Guid userId);
        List<InforDTO> GetInforByFullName(string fullname);

    }
}

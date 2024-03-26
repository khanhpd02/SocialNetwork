using SocialNetwork.DTO;

namespace SocialNetwork.Service
{
    public interface INotifyService
    {
        List<NotifyDTO> GetNotifyAcceptFriendAlongToUser();
        List<NotifyDTO> GetNotifyPostAlongToUser();
        List<NotifyDTO> GetNotifyAlongToUser();
    }
}

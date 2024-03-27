using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;

namespace SocialNetwork.Service
{
    public interface IFriendService
    {
        AppResponse SendToFriend(Guid userIdReceive);
        AppResponse AcceptFriend(Guid userIdSender);
        AppResponse UnFriend(Guid userId);
        AppResponse RefuseFriend(Guid userIdSender);
        List<InforDTO> GetAllFriends();
        List<InforDTO> GetAllNotFriends();
        List<InforDTO> GetAllFriendsRequests();
        List<MasterDatumDTO> GetAllLevel();
        FriendDTO UpdateLevelFriend(FriendDTO dto);
        //Block
        AppResponse Block(Guid userIdBlocked);
        AppResponse UnBlock(Guid userIdBlocked);
        List<InforDTO> GetListBlock();

    }
}

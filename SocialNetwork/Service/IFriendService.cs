using SocialNetwork.DTO;

namespace SocialNetwork.Service
{
    public interface IFriendService
    {
        FriendDTO SendToFriend(FriendDTO dto);
        FriendDTO AcceptFriend(FriendDTO dto);
        List<InforDTO> GetAllFriends();
        List<MasterDatumDTO> GetAllLevel();
        FriendDTO UpdateLevelFriend(FriendDTO dto);
    }
}

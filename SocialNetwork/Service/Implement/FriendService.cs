using AutoMapper;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.Entity;
using SocialNetwork.Repository;

namespace SocialNetwork.Service.Implement
{
    public class FriendService : IFriendService
    {

        private SocialNetworkContext _context;
        private readonly IFriendRepository friendRepository;
        private readonly IMasterDataRepository masterDataRepository;
        private IUserService _userService;
        private readonly INotifyRepository notifyRepository;
        private readonly IUserRepository userRepository;
        private readonly IInforRepository inforRepository;

        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public FriendService(SocialNetworkContext context, IFriendRepository friendRepository,
            IMasterDataRepository masterDataRepository, IUserService userService,
            INotifyRepository notifyRepository, IUserRepository userRepository,
            IInforRepository inforRepository)
        {
            _context = context;
            this.friendRepository = friendRepository;
            this.masterDataRepository = masterDataRepository;
            _userService = userService;
            this.notifyRepository = notifyRepository;
            this.userRepository = userRepository;
            this.inforRepository = inforRepository;
        }
        public FriendDTO SendToFriend(FriendDTO dto)
        {
            Friend friend = mapper.Map<Friend>(dto);
            friend.UserTo = _userService.UserId;
            friend.UserAccept = dto.User2;
            friend.IsDeleted = true;
            friendRepository.CreateIsTemp(friend);
            friendRepository.Save();

            Notify notify = new Notify();
            notify.UserTo = _userService.UserId;
            User user = userRepository.FindByCondition(x => x.Id == _userService.UserId).FirstOrDefault();
            Infor infor = inforRepository.FindByCondition(x => x.UserId == user.Id).FirstOrDefault();
            notify.UserNotify = dto.User2;
            var notifyType = masterDataRepository.FindByCondition(x => x.Name == "Kết bạn").FirstOrDefault();
            notify.Content = $"{infor.FullName} đã gửi lời mời kết bạn cho bạn";
            notify.NotifyType = notifyType.Id;
            notifyRepository.Create(notify);
            notifyRepository.Save();

            return dto;
        }
        public FriendDTO AcceptFriend(FriendDTO dto)
        {
            var LevelFriend = masterDataRepository.FindByCondition(x => x.Name == "Bạn thường").FirstOrDefault();

            var friends = friendRepository.FindByCondition(x => x.UserTo == dto.User1 && x.UserAccept == _userService.UserId).FirstOrDefault();
            friends.Level = LevelFriend.Id;
            friends.IsDeleted = false;
            friendRepository.Update(friends);
            friendRepository.Save();
            return dto;
        }
        public List<InforDTO> GetAllFriends()
        {
            List<Guid> idOfFriends = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) && x.IsDeleted == false)
                .Select(x => x.UserTo == _userService.UserId ? x.UserAccept : x.UserTo)
                .ToList();

            List<Infor> infors = new List<Infor>();

            foreach (var friendId in idOfFriends)
            {
                User user = userRepository.FindByCondition(x => x.Id == friendId && x.IsDeleted == false).FirstOrDefault();

                if (user != null)
                {
                    Infor infor = inforRepository.FindByCondition(x => x.UserId == user.Id).FirstOrDefault();

                    if (infor != null)
                    {
                        infors.Add(infor);
                    }
                }
            }

            List<InforDTO> inforDTOs = new List<InforDTO>();

            foreach (var infor in infors)
            {
                var level = (Guid)friendRepository.FindByCondition(x =>
                    (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) &&
                    (x.UserTo == infor.UserId || x.UserAccept == infor.UserId) &&
                    x.IsDeleted == false)
                    .Select(x => x.Level)
                    .FirstOrDefault();

                string levelName = masterDataRepository.FindByCondition(x => x.Id == level).Select(x => x.Name).FirstOrDefault();

                InforDTO inforDTO = mapper.Map<InforDTO>(infor);
                inforDTO.LevelFriend = levelName;
                inforDTOs.Add(inforDTO);
            }

            return inforDTOs;
        }

        public List<MasterDatumDTO> GetAllLevel()
        {
            List<MasterDatum> level = masterDataRepository.FindByCondition(x => x.Name == "Bạn thường" || x.Name == "Thân thiết").ToList();
            List<MasterDatumDTO> masterDatumDTOs = new List<MasterDatumDTO>();
            foreach (var item in level)
            {
                MasterDatumDTO masterDatumDTO = mapper.Map<MasterDatumDTO>(item);
                masterDatumDTOs.Add(masterDatumDTO);
            }
            return masterDatumDTOs;
        }
        public FriendDTO UpdateLevelFriend(FriendDTO dto)
        {
            var friends = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId && x.UserAccept == dto.User2) || (x.UserTo == dto.User2 && x.UserAccept == _userService.UserId)).FirstOrDefault();
            friends.Level = dto.Level;
            friendRepository.Update(friends);
            friendRepository.Save();
            return dto;
        }
    }
}

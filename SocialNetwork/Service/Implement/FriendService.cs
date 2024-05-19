using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Helpers;
using SocialNetwork.Repository;
using System.Linq;

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
        public AppResponse SendToFriend(Guid userIdReceive)
        {
            var checkFriend = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId && x.UserAccept == userIdReceive) || ((x.UserTo == userIdReceive && x.UserAccept == _userService.UserId))).FirstOrDefault();
            var checkUser = userRepository.FindByCondition(x => x.Id == userIdReceive && x.IsDeleted == false).FirstOrDefault();
            var Infor = inforRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false).FirstOrDefault();
            if (checkUser == null)
            {
                throw new BadRequestException("UserId không tồn tại");

            }
            else if (checkFriend != null)
            {
                throw new BadRequestException("Đã gửi lời mới kết bạn hoặc đã kết bạn hoặc User đã gửi lời mời cho bạn");
            }
            else
            {
                Friend friend = new Friend
                {
                    UserTo = _userService.UserId,
                    UserAccept = userIdReceive,
                    IsDeleted = true,
                    CreateBy = _userService.UserId,
                    CreateDate = DateTime.Now
                };
                friendRepository.CreateIsTemp(friend);
                friendRepository.Save();

                Notify notify = new Notify();
                notify.UserTo = _userService.UserId;
                notify.UserNotify = userIdReceive;
                var notifyType = masterDataRepository.FindByCondition(x => x.Name == "Kết bạn").FirstOrDefault();
                notify.Content = $"{Infor.FullName} đã gửi lời mời kết bạn cho bạn";
                notify.NotifyType = notifyType.Id;
                notify.CreateDate = DateTime.Now;
                notify.Image = Infor.Image;
                notifyRepository.Create(notify);
                notifyRepository.Save();
                return new AppResponse { message = "Gửi lời mời kết bạn thành công", success = true };
            }


        }
        public AppResponse AcceptFriend(Guid userIdSender)
        {
            var LevelFriend = masterDataRepository.FindByCondition(x => x.Name == "Bạn thường").FirstOrDefault();
            var userSender = userRepository.FindByCondition(x => x.Id == userIdSender && x.IsDeleted == false).FirstOrDefault();
            var friends = friendRepository.FindByCondition(x => x.UserTo == userIdSender && x.UserAccept == _userService.UserId && x.IsDeleted == true).FirstOrDefault();
            var myInfor = inforRepository.FindByCondition(x => x.UserId == _userService.UserId).FirstOrDefault();
            if (userIdSender == null)
            {
                throw new BadRequestException("UserId không tồn tại");
            }
            else if (friends == null)
            {
                throw new BadRequestException("Không có lời mời kết bạn từ userID hoặc đã kết bạn");
            }
            else
            {
                friends.Level = LevelFriend.Id;
                friends.IsDeleted = false;
                friendRepository.Update(friends);
                friendRepository.Save();
                var notifyType = masterDataRepository.FindByCondition(x => x.Name == "AcceptFriend").FirstOrDefault();
                Notify notify = new Notify
                {
                    UserTo = _userService.UserId,
                    UserNotify = userIdSender,
                    Content = $" đã chấp nhận lời mời kết bạn",
                    NotifyType = notifyType.Id,
                    CreateDate = DateTime.Now,
                    CreateBy = _userService.UserId
                };
                notifyRepository.Create(notify);
                notifyRepository.Save();
                return new AppResponse { message = "Accept Friend Success", success = true };
            }


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
            List<MasterDatum> level = masterDataRepository.FindByCondition(x => x.Name == masterDataRepository.GetEnumDescription(EnumLevelView.friend) || x.Name == masterDataRepository.GetEnumDescription(EnumLevelView.bestfriend)).ToList();
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
            Guid? idMaster=null;
            var enumDescription = dto.Level == (int)EnumLevelView.friend ? masterDataRepository.GetEnumDescription(EnumLevelView.friend) :
                     dto.Level == (int)EnumLevelView.bestfriend ? masterDataRepository.GetEnumDescription(EnumLevelView.bestfriend) :
                     null;

            if (enumDescription != null)
            {
                idMaster = masterDataRepository.FindByCondition(x => x.Name == enumDescription && !x.IsDeleted).FirstOrDefault()?.Id;
            }
            friends.Level = idMaster;
            friendRepository.Update(friends);
            friendRepository.Save();
            return dto;
        }

        public List<InforDTO> GetAllFriendsRequests()
        {

            List<Guid> idOfFriend = friendRepository.FindByCondition(x => x.UserAccept == _userService.UserId && x.IsDeleted == true)
                .Select(x => x.UserTo)
                .ToList();
            List<Infor> infors = new List<Infor>();

            foreach (var friendId in idOfFriend)
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


                InforDTO inforDTO = mapper.Map<InforDTO>(infor);

                inforDTOs.Add(inforDTO);
            }

            return inforDTOs;


        }

        public AppResponse UnFriend(Guid userId)
        {
            var checkFriend = friendRepository.FindByCondition(x => ((x.UserTo == _userService.UserId && x.UserAccept == userId) || (x.UserTo == userId && x.UserAccept == _userService.UserId)) && x.IsDeleted == false).FirstOrDefault();
            //var LevelFriend = masterDataRepository.FindByCondition(x => x.Name == "Bạn thường").FirstOrDefault();
            var checkUserId = userRepository.FindByCondition(x => x.Id == userId && x.IsDeleted == false).FirstOrDefault();
            //var friends = friendRepository.FindByCondition(x => x.UserTo == userIdSender && x.UserAccept == _userService.UserId && x.IsDeleted == false).FirstOrDefault();
            //var inforSender = inforRepository.FindByCondition(x => x.UserId == userIdSender);
            if (checkUserId == null)
            {
                throw new BadRequestException("UserId không tồn tại");
            }
            else if (checkFriend == null)
            {
                throw new BadRequestException("Chưa kết bạn sao unfriend");
            }
            else
            {
                friendRepository.Delete(checkFriend);
                friendRepository.Save();


                return new AppResponse { message = "UnFriend Success", success = true };
            }
        }

        public AppResponse RefuseFriend(Guid userIdSender)
        {
            var userSender = userRepository.FindByCondition(x => x.Id == userIdSender && x.IsDeleted == false).FirstOrDefault();
            var friends = friendRepository.FindByCondition(x => x.UserTo == userIdSender && x.UserAccept == _userService.UserId && x.IsDeleted == true).FirstOrDefault();
            if (userIdSender == null)
            {
                throw new BadRequestException("UserId không tồn tại");
            }
            else if (friends == null)
            {
                throw new BadRequestException("Không có lời mời kết bạn từ userID hoặc đã kết bạn");
            }
            else
            {
                friendRepository.Delete(friends);
                friendRepository.Save();

                return new AppResponse { message = "Reject Friend Success", success = true };
            }
        }

        public List<InforDTO> GetAllNotFriends()
        {
            List<Guid> idOfFriends = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) && x.IsDeleted == false)
                .Select(x => x.UserTo == _userService.UserId ? x.UserAccept : x.UserTo)
                .ToList();
            var userSuggest = userRepository
                .FindByCondition(x => x.IsDeleted == false && !idOfFriends.Contains(x.Id) && x.Id != _userService.UserId)
                .ToList();
            List<User> listUserSuggest = new List<User>();
            var itemsToRemove = new List<User>();
            foreach (var item in userSuggest)
            {
                List<Guid> idOfFriendsSuggest = friendRepository.FindByCondition(x => (x.UserTo == item.Id || x.UserAccept == item.Id) && x.IsDeleted == false)
                .Select(x => x.UserTo == item.Id ? x.UserAccept : x.UserTo)
                .ToList();
                if (idOfFriends.Intersect(idOfFriendsSuggest).Any())
                {
                    listUserSuggest.Add(item);
                    itemsToRemove.Add(item);
                }
            }
            foreach (var item in itemsToRemove)
            {
                userSuggest.Remove(item);
            }
            var userSuggestIds = userSuggest.Select(u => u.Id).ToList();
            var inforSuggest = inforRepository
                .FindByCondition(x => !x.IsDeleted && userSuggestIds.Contains(x.UserId.Value))
                .ToList();
            var myInfor = inforRepository.FindByCondition(x => !x.IsDeleted && x.UserId == _userService.UserId).FirstOrDefault();
            var itemsToRemoveWards = new List<Infor>();
            foreach (var item in inforSuggest)
            {
                if (item.Wards == myInfor.Wards)
                {
                    var userItem = userRepository.FindByCondition(x => x.Id == item.UserId).FirstOrDefault();
                    listUserSuggest.Add(userItem);
                    itemsToRemoveWards.Add(item);
                }
            }
            foreach (var item in itemsToRemoveWards)
            {
                inforSuggest.Remove(item);
            }
            var itemsToRemoveDistricts = new List<Infor>();
            foreach (var item in inforSuggest)
            {
                if (item.Districts == myInfor.Districts)
                {
                    var userItem = userRepository.FindByCondition(x => x.Id == item.UserId).FirstOrDefault();
                    listUserSuggest.Add(userItem);
                    itemsToRemoveDistricts.Add(item);
                }
            }
            foreach (var item in itemsToRemoveDistricts)
            {
                inforSuggest.Remove(item);
            }
            var itemsToRemoveProvinces = new List<Infor>();
            foreach (var item in inforSuggest)
            {
                if (item.Provinces == myInfor.Provinces)
                {
                    var userItem = userRepository.FindByCondition(x => x.Id == item.UserId).FirstOrDefault();
                    listUserSuggest.Add(userItem);
                    itemsToRemoveProvinces.Add(item);
                }
            }
            foreach (var item in itemsToRemoveProvinces)
            {
                inforSuggest.Remove(item);
            }
            foreach (var item in inforSuggest)
            {
                var userItem = userRepository.FindByCondition(x => x.Id == item.UserId).FirstOrDefault();
                listUserSuggest.Add(userItem);
            }
            List<Infor> listInforSuggest = new List<Infor>();
            foreach (var item in listUserSuggest)
            {
                var iteminfor = inforRepository.FindByCondition(x => x.UserId == item.Id).FirstOrDefault();
                if (iteminfor != null)
                {
                    listInforSuggest.Add(iteminfor);
                }
            }
            List<InforDTO> listInforDTO = new List<InforDTO>();
            foreach (var item in listInforSuggest)
            {
                InforDTO inforDTO = mapper.Map<InforDTO>(item);
                listInforDTO.Add(inforDTO);
            }
            return listInforDTO;
        }

        public AppResponse Block(Guid userIdBlocked)
        {
            var checkBlook = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId && x.UserAccept == userIdBlocked) || ((x.UserTo == userIdBlocked && x.UserAccept == _userService.UserId))).FirstOrDefault();
            var checkUser = userRepository.FindByCondition(x => x.Id == userIdBlocked && x.IsDeleted == false).FirstOrDefault();
            var materdata= masterDataRepository.FindByCondition(x=>x.Name==masterDataRepository.GetEnumDescription(EnumLevelView.Block)).FirstOrDefault();  
            if (checkUser == null)
            {
                throw new BadRequestException("UserId không tồn tại");

            }
            else if (checkBlook != null)
            {
                checkBlook.Level = materdata.Id;
                friendRepository.Update(checkBlook);
                friendRepository.Save();
                return new AppResponse { message = "Block success", success = true };
            }
            else
            {
                Friend friend = new Friend
                {
                    UserTo = _userService.UserId,
                    UserAccept = userIdBlocked,
                    IsDeleted = false,
                    CreateBy = _userService.UserId,
                    CreateDate = DateTime.Now,
                    Level = materdata.Id
                   
                };
                friendRepository.Create(friend);
                friendRepository.Save();

               
                return new AppResponse { message = "Block success", success = true };
            }
        }

        public List<InforDTO> GetListBlock()
        {
            var masterdataBlock= masterDataRepository.FindByCondition(x=>x.Name==masterDataRepository.GetEnumDescription(EnumLevelView.Block)).FirstOrDefault();  

            List<Guid> idOfFriend = friendRepository.FindByCondition(x => x.UserTo == _userService.UserId && x.IsDeleted == false && x.Level==masterdataBlock.Id)
                .Select(x => x.UserAccept)
                .ToList();
            List<Infor> infors = new List<Infor>();

            foreach (var friendId in idOfFriend)
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


                InforDTO inforDTO = mapper.Map<InforDTO>(infor);

                inforDTOs.Add(inforDTO);
            }

            return inforDTOs;
        }

        public AppResponse UnBlock(Guid userIdBlocked)
        {
            var checkBlook = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId && x.UserAccept == userIdBlocked) || ((x.UserTo == userIdBlocked && x.UserAccept == _userService.UserId))).FirstOrDefault();
            var checkUser = userRepository.FindByCondition(x => x.Id == userIdBlocked && x.IsDeleted == false).FirstOrDefault();
            var masterdata = masterDataRepository.FindByCondition(x => x.Name == masterDataRepository.GetEnumDescription(EnumLevelView.Block)).FirstOrDefault();
            if (checkUser == null)
            {
                throw new BadRequestException("UserId không tồn tại");

            }
            else if (checkBlook != null)
            {
                if (checkBlook.Level==masterdata.Id)
                {
                    friendRepository.Delete(checkBlook);
                    friendRepository.Save();
                    return new AppResponse { message = "Unblock success", success = true };

                }
                else
                {
                    throw new BadRequestException("UserId này không có trong danh sách chặn");

                }
                
            }
            else
            {
                throw new BadRequestException("UserId này không có trong danh sách chặn");
            }
        }
    }
}

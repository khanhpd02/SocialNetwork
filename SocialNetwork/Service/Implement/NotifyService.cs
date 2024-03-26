using AutoMapper;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.Entity;
using SocialNetwork.Repository;

namespace SocialNetwork.Service.Implement
{
    public class NotifyService : INotifyService
    {

        private SocialNetworkContext _context;
        private readonly IFriendRepository friendRepository;
        private readonly IMasterDataRepository masterDataRepository;
        private IUserService _userService;
        private readonly INotifyRepository notifyRepository;

        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public NotifyService(SocialNetworkContext context, IFriendRepository friendRepository,
            IMasterDataRepository masterDataRepository, IUserService userService,
            INotifyRepository notifyRepository)
        {
            _context = context;
            this.friendRepository = friendRepository;
            this.masterDataRepository = masterDataRepository;
            _userService = userService;
            this.notifyRepository = notifyRepository;
        }
        public List<NotifyDTO> GetNotifyAcceptFriendAlongToUser()
        {
            var notifyType = masterDataRepository.FindByCondition(x => x.Name == "Kết bạn").FirstOrDefault();

            List<Notify> notify = notifyRepository.FindByCondition(x => x.UserNotify == _userService.UserId && x.NotifyType == notifyType.Id).ToList();
            List<NotifyDTO> notifyDTOs = new List<NotifyDTO>();
            foreach (var item in notify)
            {
                NotifyDTO dto = mapper.Map<NotifyDTO>(item);
                notifyDTOs.Add(dto);
            }
            return notifyDTOs;
        }
        public List<NotifyDTO> GetNotifyPostAlongToUser()
        {
            var notifyType = masterDataRepository.FindByCondition(x => x.Name == "Đăng post").FirstOrDefault();
            List<Notify> notify = notifyRepository.FindByCondition(x => x.UserNotify == _userService.UserId && x.NotifyType == notifyType.Id).ToList();
            List<NotifyDTO> notifyDTOs = new List<NotifyDTO>();
            foreach (var item in notify)
            {
                NotifyDTO dto = mapper.Map<NotifyDTO>(item);
                notifyDTOs.Add(dto);
            }
            return notifyDTOs;
        }
        public List<NotifyDTO> GetNotifyCommentAlongToUser()
        {
            var notifyType = masterDataRepository.FindByCondition(x => x.Name == "Bình luận").FirstOrDefault();
            List<Notify> notify = notifyRepository.FindByCondition(x => x.UserNotify == _userService.UserId && x.NotifyType == notifyType.Id).ToList();
            List<NotifyDTO> notifyDTOs = new List<NotifyDTO>();
            foreach (var item in notify)
            {
                NotifyDTO dto = mapper.Map<NotifyDTO>(item);
                notifyDTOs.Add(dto);
            }
            return notifyDTOs;
        }
        public List<NotifyDTO> GetNotifyAlongToUser()
        {
            var notifyPost = GetNotifyPostAlongToUser();
            var notifyComment = GetNotifyCommentAlongToUser();

            var combinedNotifications = notifyPost.Concat(notifyComment);

            return combinedNotifications.OrderByDescending(dto => dto.CreateDate).ToList();
        }
    }
}

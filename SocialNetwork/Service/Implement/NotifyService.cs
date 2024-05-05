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
        private readonly IPostRepository postRepository;
        private readonly ICommentRepository commentRepository;

        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public NotifyService(SocialNetworkContext context, IFriendRepository friendRepository,
            IMasterDataRepository masterDataRepository, IUserService userService,
            INotifyRepository notifyRepository, IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _context = context;
            this.friendRepository = friendRepository;
            this.masterDataRepository = masterDataRepository;
            _userService = userService;
            this.notifyRepository = notifyRepository;
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
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
                dto.PostId = item.IdObject;
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
                var comment = commentRepository.FindByCondition(x => x.IsDeleted == false && x.Id == item.IdObject).FirstOrDefault();
                if (comment!=null)
                {
                    var post = postRepository.FindByCondition(x => x.IsDeleted == false && x.Id == comment.PostId).FirstOrDefault();
                    NotifyDTO dto = mapper.Map<NotifyDTO>(item);
                    dto.PostId = post.Id;
                    dto.CommentId = item.IdObject;
                    notifyDTOs.Add(dto);
                }
                
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

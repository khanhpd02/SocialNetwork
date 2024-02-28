using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Helpers;
using SocialNetwork.Repository;
using Video = SocialNetwork.Entity.Video;

namespace SocialNetwork.Service.Implement
{
    public class PostService : IPostService
    {
        private readonly Cloudinary _cloudinary;
        private SocialNetworkContext _context;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly IImageRepository imageRepository;
        private readonly IVideoRepository videoRepository;
        private readonly ILikeRepository likeRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IFriendRepository friendRepository;
        private readonly IMasterDataRepository masterDataRepository;
        private readonly INotifyRepository notifyRepository;
        private readonly IInforRepository inforRepository;

        private IUserService _userService;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public PostService(IPostRepository postRepository, IUserRepository userRepository, IImageRepository imageRepository,
            SocialNetworkContext _context, Cloudinary _cloudinary,
            IVideoRepository videoRepository, ILikeRepository likeRepository, ICommentRepository commentRepository,
            IUserService userService, IFriendRepository friendRepository,
            IMasterDataRepository masterDataRepository, INotifyRepository notifyRepository, IInforRepository inforRepository)
        {
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.imageRepository = imageRepository;
            this._context = _context;
            this._cloudinary = _cloudinary;
            this.videoRepository = videoRepository;
            this.likeRepository = likeRepository;
            this.commentRepository = commentRepository;
            _userService = userService;
            this.friendRepository = friendRepository;
            this.masterDataRepository = masterDataRepository;
            this.notifyRepository = notifyRepository;
            this.inforRepository = inforRepository;
        }
        public List<string> UploadFilesToCloudinary(List<IFormFile> files)
        {
            List<string> uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    if (Path.GetExtension(file.FileName).Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                    {
                        var uploadParamsVideo = new VideoUploadParams
                        {
                            File = new FileDescription(file.FileName, file.OpenReadStream()),
                            Folder = "SocialNetwork/Video/",
                        };

                        try
                        {
                            var uploadResult = _cloudinary.Upload(uploadParamsVideo);
                            uploadedUrls.Add(uploadResult.SecureUrl.AbsoluteUri);
                        }
                        catch (Exception)
                        {
                            // Handle error
                        }
                    }
                    else if (Path.GetExtension(file.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                             Path.GetExtension(file.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                             Path.GetExtension(file.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                             Path.GetExtension(file.FileName).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(file.FileName, file.OpenReadStream()),
                            Folder = "SocialNetwork/Image/",
                        };

                        try
                        {
                            var uploadResult = _cloudinary.Upload(uploadParams);
                            uploadedUrls.Add(uploadResult.SecureUrl.AbsoluteUri);
                        }
                        catch (Exception)
                        {
                            // Handle error
                        }
                    }
                }
            }

            return uploadedUrls;
        }

        public PostDTO Create(PostDTO dto)
        {
            List<string> cloudinaryUrls = UploadFilesToCloudinary(dto.File);

            Post post = mapper.Map<Post>(dto);
            post.UserId = _userService.UserId;
            postRepository.Create(post);
            postRepository.Save();

            foreach (var cloudinaryUrl in cloudinaryUrls)
            {
                if (!string.IsNullOrEmpty(cloudinaryUrl))
                {
                    string fileExtension = Path.GetExtension(cloudinaryUrl);
                    if (fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        fileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                        fileExtension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
                    {
                        var image = new Image
                        {
                            PostId = post.Id,
                            LinkImage = cloudinaryUrl,
                            CreateDate = DateTime.Now,
                            CreateBy = _userService.UserId,
                            IsDeleted = false
                        };
                        imageRepository.Create(image);
                        imageRepository.Save();
                    }
                    else if (fileExtension.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                    {
                        var video = new Video
                        {
                            PostId = post.Id,
                            Link = cloudinaryUrl,
                            CreateDate = DateTime.Now,
                            CreateBy = _userService.UserId,
                            IsDeleted = false
                        };
                        videoRepository.Create(video);
                        videoRepository.Save();
                    }
                }
            }
            var friendType = masterDataRepository.FindByCondition(x => x.Name == "Thân thiết").FirstOrDefault();
            List<Friend> friends = friendRepository.FindByCondition(x => x.UserTo == _userService.UserId && x.Level == friendType.Id).ToList();
            foreach (var item in friends)
            {
                Notify notify = new Notify();
                notify.UserTo = _userService.UserId;
                User user = userRepository.FindByCondition(x => x.Id == _userService.UserId).FirstOrDefault();
                Infor infor = inforRepository.FindByCondition(x => x.UserId == user.Id).FirstOrDefault();
                notify.UserNotify = item.UserAccept;
                var notifyType = masterDataRepository.FindByCondition(x => x.Name == "Đăng post").FirstOrDefault();
                notify.Content = $"{infor.FullName} đã đăng bài post";
                notify.NotifyType = notifyType.Id;
                notifyRepository.Create(notify);
                notifyRepository.Save();
            }
            return dto;
        }
        public PostDTO Update(PostDTO dto)
        {
            var postcheck = postRepository.FindByCondition(x => x.Id == dto.Id).FirstOrDefault();
            if (postcheck == null)
            {
                throw new PostNotFoundException(dto.Id);
            }

            // Xác định ảnh cuối cùng liên kết với bài đăng và đánh dấu nó là đã xóa
            var imageLast = imageRepository.FindByCondition(x => x.PostId == dto.Id).FirstOrDefault();
            if (dto.Images != null && imageLast != null)
            {
                imageLast.IsDeleted = true;
                imageRepository.Update(imageLast);
                imageRepository.Save();
            }

            // Cập nhật nội dung bài đăng
            postcheck.Content = dto.Content;
            postRepository.Update(postcheck);
            postRepository.Save();

            // Tải lên và liên kết các ảnh/video mới (nếu có)
            if (dto.File != null && dto.File.Any())
            {
                List<string> cloudinaryUrls = UploadFilesToCloudinary(dto.File);

                foreach (var cloudinaryUrl in cloudinaryUrls)
                {
                    if (!string.IsNullOrEmpty(cloudinaryUrl))
                    {
                        string fileExtension = Path.GetExtension(cloudinaryUrl);
                        if (fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            fileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                            fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                            fileExtension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
                        {
                            var link = cloudinaryUrl;
                            Image image = new Image
                            {
                                PostId = dto.Id,
                                LinkImage = link,
                                CreateDate = DateTime.Now,
                                CreateBy = _userService.UserId,
                                IsDeleted = false
                            };
                            imageRepository.Create(image);
                            imageRepository.Save();
                        }
                        else if (fileExtension.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                        {
                            var link = cloudinaryUrl;
                            Video video = new Video
                            {
                                PostId = dto.Id,
                                Link = link,
                                CreateDate = DateTime.Now,
                                CreateBy = _userService.UserId,
                                IsDeleted = false
                            };
                            videoRepository.Create(video);
                            videoRepository.Save();
                        }
                    }
                }
            }

            return dto;
        }

        public PostDTO GetById(Guid id)
        {
            var CountLike = 0;
            var CountComment = 0;
            Post entity = postRepository.FindById(id) ?? throw new PostNotFoundException(id);
            var infor = inforRepository.FindByCondition(x => x.UserId == entity.UserId && x.IsDeleted == false).FirstOrDefault();

            List<Image> images = imageRepository.FindByCondition(img => img.PostId == id && img.IsDeleted == false).ToList();
            List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == id && vid.IsDeleted == false).ToList();
            List<Like> likes = likeRepository.FindByCondition(img => img.PostId == id && img.IsDeleted == false).ToList();
            List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == id && vid.IsDeleted == false).ToList();
            CountLike = likes.Count();
            CountComment = comments.Count();
            PostDTO dto = mapper.Map<PostDTO>(entity);
            dto.FullName=infor.FullName;
            dto.AvatarUrl = infor.Image;
            dto.Images = images;
            dto.Videos = videos;
            dto.Likes = likes;
            dto.Comments = comments;
            dto.CountLike = CountLike;
            dto.CountComment = CountComment;

            return dto;
        }
        public List<PostDTO> GetPostByUserId(Guid id)
        {
            List<Post> entityList = postRepository.FindByCondition(x => x.UserId == id && x.IsDeleted == false).ToList() ?? throw new PostNotFoundException(id);

            List<PostDTO> dtoList = new List<PostDTO>();
            var CountLike = 0;
            var CountComment = 0;

            foreach (Post entity in entityList)
            {
                var infor= inforRepository.FindByCondition(x=>x.UserId==entity.UserId && x.IsDeleted == false).FirstOrDefault();
                List<Image> images = imageRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                List<Like> likes = likeRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                CountLike = likes.Count();
                CountComment = comments.Count();
                PostDTO dto = mapper.Map<PostDTO>(entity);
                dto.AvatarUrl = infor.Image;
                dto.FullName= infor.FullName;
                dto.Images = images;
                dto.Videos = videos;
                dto.Likes = likes;
                dto.Comments = comments;
                dto.CountLike = CountLike;
                dto.CountComment = CountComment;

                dtoList.Add(dto);
            }
            return dtoList;
        }
        public void Delete(Guid id)
        {
            if (postRepository.FindById(id) == null)
                throw new PostNotFoundException(id);

            DeletePostAndRelationShip(id);

            postRepository.Save();
        }
        public List<PostDTO> GetAll()
        {
            List<Guid> idOfFriends = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) && x.IsDeleted == false)
                .Select(x => x.UserTo == _userService.UserId ? x.UserAccept : x.UserTo)
                .ToList();
            List<Post> entityList = postRepository.FindAll();
            List<PostDTO> dtoList = new List<PostDTO>();
            var CountLike = 0;
            var CountComment = 0;
            List<Post> postsToRemove = new List<Post>();
            foreach (Post post in entityList)
            {
                int has = 0;
                if (post.UserId == _userService.UserId && post.IsDeleted == false)
                {
                    continue;
                }
                if (idOfFriends.Count != 0)
                {
                    foreach (Guid idfriend in idOfFriends)
                    {
                        if ((idfriend == post.UserId || post.LevelView == (int)(EnumLevelView.publicview)) && post.IsDeleted == false)
                        {
                            has = 1; break;
                        }
                    }
                    if (has == 0)
                    {
                        postsToRemove.Add(post);
                    }
                }
                else if (post.LevelView == 2)
                { postsToRemove.Add(post); }
            }

            foreach (var postToRemove in postsToRemove)
            {
                entityList.Remove(postToRemove);
            }
            foreach (Post entity in entityList)
            {
                var infor =inforRepository.FindByCondition(x=>x.UserId== entity.UserId && x.IsDeleted == false).FirstOrDefault();
                var like = likeRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false && x.PostId == entity.Id).FirstOrDefault();
                List<Image> images = imageRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                List<Like> likes = likeRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                CountLike = likes.Count();
                CountComment = comments.Count();
                PostDTO dto = mapper.Map<PostDTO>(entity);
                dto.FullName=infor.FullName;
                dto.AvatarUrl = infor.Image;
                dto.Images = images;
                dto.Videos = videos;
                dto.Likes = likes;
                dto.Comments = comments;
                dto.CountLike = CountLike;
                dto.CountComment = CountComment;
                if (like == null)
                {
                    dto.islike = false;
                }
                else
                {
                    dto.islike = true;
                }

                dtoList.Add(dto);
            }

            return dtoList;
        }
        private void DeletePostAndRelationShip(Guid id)
        {
            Post post = postRepository.FindByConditionWithTracking(x => x.Id == id, x => x.Comments, x => x.Images, x => x.Likes, x => x.Reports, x => x.TagPosts, x => x.Videos).FirstOrDefault()!;
            post.IsDeleted = true;

        }

    }
}

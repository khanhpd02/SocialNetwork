﻿using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
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
        public string UploadFileToCloudinary(IFormFile fileUploadDTO)
        {
            if (fileUploadDTO != null && fileUploadDTO.Length > 0)
            {
                if (Path.GetExtension(fileUploadDTO.FileName).Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                {
                    var uploadParamsVideo = new VideoUploadParams
                    {
                        File = new FileDescription(fileUploadDTO.FileName, fileUploadDTO.OpenReadStream()),
                        Folder = "SocialNetwork/Video/",
                    };

                    try
                    {
                        var uploadResult = _cloudinary.Upload(uploadParamsVideo);
                        return uploadResult.SecureUrl.AbsoluteUri;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                if (Path.GetExtension(fileUploadDTO.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(fileUploadDTO.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(fileUploadDTO.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(fileUploadDTO.FileName).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(fileUploadDTO.FileName, fileUploadDTO.OpenReadStream()),
                        Folder = "SocialNetwork/Image/",
                    };

                    try
                    {
                        var uploadResult = _cloudinary.Upload(uploadParams);
                        return uploadResult.SecureUrl.AbsoluteUri;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }

            }

            return null;
        }
        public PostDTO Create(PostDTO dto)
        {

            string cloudinaryUrl = UploadFileToCloudinary(dto.File);
            Post post = mapper.Map<Post>(dto);
            post.UserId = _userService.UserId;
            postRepository.Create(post);
            postRepository.Save();

            if (cloudinaryUrl != null && cloudinaryUrl.Length > 0)
            {
                string fileExtension = Path.GetExtension(cloudinaryUrl);
                if (fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                       fileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                       fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                       fileExtension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
                {
                    var link = cloudinaryUrl;
                    if (link != null)
                    {
                        Image image = new Image
                        {
                            PostId = post.Id,
                            LinkImage = link,
                            CreateDate = DateTime.Now,
                            CreateBy = _userService.UserId,
                            IsDeleted = false
                        };
                        imageRepository.Create(image);
                        imageRepository.Save();
                    }
                }
                if (fileExtension.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                {
                    var link = cloudinaryUrl;
                    if (link != null)
                    {
                        Video video = new Video
                        {
                            PostId = post.Id,
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
            string cloudinaryUrl = UploadFileToCloudinary(dto.File);

            postcheck.Content = dto.Content;
            postRepository.Update(postcheck);
            postRepository.Save();
            if (cloudinaryUrl != null && cloudinaryUrl.Length > 0)
            {
                string fileExtension = Path.GetExtension(cloudinaryUrl);
                if (fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                       fileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                       fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                       fileExtension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
                {
                    var link = cloudinaryUrl;
                    if (link != null)
                    {
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
                }
                if (fileExtension.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                {
                    var link = cloudinaryUrl;
                    if (link != null)
                    {
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

            return dto;
        }
        public PostDTO GetById(Guid id)
        {
            var CountLike = 0;
            var CountComment = 0;
            Post entity = postRepository.FindById(id) ?? throw new PostNotFoundException(id);

            List<Image> images = imageRepository.FindByCondition(img => img.PostId == id && img.IsDeleted == false).ToList();
            List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == id && vid.IsDeleted == false).ToList();
            List<Like> likes = likeRepository.FindByCondition(img => img.PostId == id && img.IsDeleted == false).ToList();
            List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == id && vid.IsDeleted == false).ToList();
            CountLike = likes.Count();
            CountComment = comments.Count();
            PostDTO dto = mapper.Map<PostDTO>(entity);

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
                List<Image> images = imageRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                List<Like> likes = likeRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                CountLike = likes.Count();
                CountComment = comments.Count();
                PostDTO dto = mapper.Map<PostDTO>(entity);
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
            foreach (Post post in entityList)
            {
                int has = 0;
                if (idOfFriends.Count != 0)
                {
                    foreach (Guid idfriend in idOfFriends)
                    {
                        if ((idfriend == post.UserId || post.LevelView == 1) && post.IsDeleted == false)
                        {
                            has = 1; break;
                        }
                    }
                    if (has == 0)
                    {
                        entityList.Remove(post);
                    }
                }
                else if (post.LevelView == 2)
                { entityList.Remove(post); }

            }


            foreach (Post entity in entityList)
            {


                var like = likeRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false && x.PostId == entity.Id).FirstOrDefault();

                List<Image> images = imageRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                List<Like> likes = likeRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                CountLike = likes.Count();
                CountComment = comments.Count();
                PostDTO dto = mapper.Map<PostDTO>(entity);
                dto.Images = images;
                dto.Videos = videos;
                dto.Likes = likes;
                dto.Comments = comments;
                dto.CountLike = CountLike;
                dto.CountComment = CountComment;
                //dto.LevelView=entity.LevelView; 
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

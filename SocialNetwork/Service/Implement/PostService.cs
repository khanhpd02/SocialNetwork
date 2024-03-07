using AngleSharp.Dom;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DocumentFormat.OpenXml.Spreadsheet;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Helpers;
using SocialNetwork.Repository;
using Comment = SocialNetwork.Entity.Comment;
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
        private readonly IShareRepository shareRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private IUserService _userService;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public PostService(IPostRepository postRepository, IUserRepository userRepository, IImageRepository imageRepository,
            SocialNetworkContext _context, Cloudinary _cloudinary,
            IVideoRepository videoRepository, ILikeRepository likeRepository, ICommentRepository commentRepository,
            IUserService userService, IFriendRepository friendRepository,
            IMasterDataRepository masterDataRepository, INotifyRepository notifyRepository, IInforRepository inforRepository,
            IShareRepository shareRepository, IHttpContextAccessor httpContextAccessor)
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
            this.shareRepository = shareRepository;
            this.httpContextAccessor = httpContextAccessor;
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
        public ShareDTO SharePost(ShareDTO sharePostDTO)
        {
            var postOrigin = postRepository.FindByCondition(x=>x.Id==sharePostDTO.PostId).FirstOrDefault();
            var currentDomain = httpContextAccessor.HttpContext.Request.Host.Value;
            if (postOrigin == null)
            {
                throw new Exception("Id of Post is invalid");
            }
            else if (postOrigin != null && postOrigin.IsDeleted == false && (postOrigin.LevelView == (int)(EnumLevelView.publicview) || postOrigin.LevelView == (int)(EnumLevelView.friendview)))
            {
                Share share = new Share
                {
                    Content = sharePostDTO.Content,
                    PostId = sharePostDTO.PostId,
                    UserId = _userService.UserId,
                    //Link = $"https://{currentDomain}/api/post/{postId}",
                    LevelView = sharePostDTO.LevelView
                };
                shareRepository.Create(share);
                shareRepository.Save();
                ShareDTO shareDTO = mapper.Map<ShareDTO>(share);
                return shareDTO;
            }
            else
            {
                throw new Exception("Level view of Post is not accept share");
            }
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
            dto.FullName = infor.FullName;
            dto.AvatarUrl = infor.Image;
            dto.Images = images;
            dto.Videos = videos;
            dto.Likes = likes;
            dto.Comments = comments;
            dto.CountLike = CountLike;
            dto.CountComment = CountComment;

            return dto;
        }
        public List<PostDTO> GetAllPostShare()
        {
            List<Guid> idOfFriends = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) && x.IsDeleted == false)
                .Select(x => x.UserTo == _userService.UserId ? x.UserAccept : x.UserTo)
                .ToList();
            List<PostDTO> postsToRemove = new List<PostDTO>();
            var shares = shareRepository.FindByCondition(x => x.IsDeleted == false).ToList();
            var listPostShareDTO = new List<PostDTO>();
            var CountLike = 0;
            var CountComment = 0;
            var CountLikeShare = 0;
            var CountCommentShare = 0;
            foreach (var share in shares)
            {
                var infor = inforRepository.FindByCondition(x => x.UserId == share.UserId).FirstOrDefault();

                var postShare = postRepository.FindByCondition(x => x.Id == share.PostId && x.IsDeleted == false).FirstOrDefault();

                if (postShare != null)
                {
                    var like = likeRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false && x.PostId == share.Id).FirstOrDefault();

                    var userShare = userRepository.FindByCondition(x => x.Id == postShare.UserId).FirstOrDefault();
                    var inforUserPost = inforRepository.FindByCondition(x => x.UserId == userShare.Id).FirstOrDefault();
                    List<Image> images = imageRepository.FindByCondition(img => img.PostId == postShare.Id && img.IsDeleted == false).ToList();
                    List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == postShare.Id && vid.IsDeleted == false).ToList();
                    List<Like> likes = likeRepository.FindByCondition(img => img.PostId == postShare.Id && img.IsDeleted == false).ToList();
                    List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == postShare.Id && vid.IsDeleted == false).ToList();
                    CountLike = likes.Count();
                    CountComment = comments.Count(); 

                    List<Like> likesShare = likeRepository.FindByCondition(img => img.PostId == share.Id && img.IsDeleted == false).ToList();
                    List<Comment> commentsShare = commentRepository.FindByCondition(vid => vid.PostId == share.Id && vid.IsDeleted == false).ToList();
                    CountLikeShare = likes.Count();
                    CountCommentShare = comments.Count();

                    var postShareDTO = mapper.Map<PostDTO>(postShare);
                    postShareDTO.FullName = inforUserPost.FullName;
                    postShareDTO.AvatarUrl = inforUserPost.Image;
                    postShareDTO.FullNameShare = infor.FullName;
                    postShareDTO.AvatarUrlShare = infor.Image;
                    postShareDTO.Images = images;
                    postShareDTO.Videos = videos;
                    postShareDTO.Likes = likes;
                    postShareDTO.Comments = comments;
                    postShareDTO.CountLike = CountLike;
                    postShareDTO.CountComment = CountComment; 
                    postShareDTO.LikesShare = likesShare;
                    postShareDTO.CommentsShare = commentsShare;
                    postShareDTO.CountLikeShare = CountLikeShare;
                    postShareDTO.CountCommentShare = CountCommentShare;
                    postShareDTO.LevelViewShare = share.LevelView;
                    postShareDTO.UserIdSharePost = infor.UserId;
                    postShareDTO.CreateDateShare = share.CreateDate;
                    postShareDTO.IdShare = share.Id;
                    if (like == null)
                    {
                        postShareDTO.islikeShare = false;
                    }
                    else
                    {
                        postShareDTO.islikeShare = true;
                    }
                    listPostShareDTO.Add(postShareDTO);
                }
            }
            foreach (PostDTO post in listPostShareDTO)
            {
                int has = 0;
                if (post.UserIdSharePost == _userService.UserId && post.IsDeleted == false)
                {
                    continue;
                }
                if (idOfFriends.Count != 0)
                {
                    foreach (Guid idfriend in idOfFriends)
                    {
                        if ((idfriend == post.UserIdSharePost || post.LevelViewShare == (int)(EnumLevelView.publicview)) && post.IsDeleted == false)
                        {
                            has = 1; break;
                        }
                    }
                    if (has == 0)
                    {
                        postsToRemove.Add(post);
                    }
                }
                else if (post.LevelViewShare == (int)(EnumLevelView.friendview))
                { postsToRemove.Add(post); }
            }

            foreach (var postToRemove in postsToRemove)
            {
                listPostShareDTO.Remove(postToRemove);
            }
            return listPostShareDTO;
        }
        public List<PostDTO> GetPostShareByUserId(Guid userId)
        {
            List<Guid> idOfFriends = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) && x.IsDeleted == false)
                .Select(x => x.UserTo == _userService.UserId ? x.UserAccept : x.UserTo)
                .ToList();
            List<PostDTO> postsToRemove = new List<PostDTO>();
            var user = userRepository.FindById(userId);
            var infor = inforRepository.FindByCondition(x => x.UserId == user.Id).FirstOrDefault();
            var shares = shareRepository.FindByCondition(x => x.UserId == user.Id).ToList();
            var listPostShareDTO = new List<PostDTO>();
            var CountLike = 0;
            var CountComment = 0;
            var CountLikeShare = 0;
            var CountCommentShare = 0;
            foreach (var share in shares)
            {
                var like = likeRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false && x.PostId == share.Id).FirstOrDefault();

                var postShare = postRepository.FindByCondition(x => x.Id == share.PostId && x.IsDeleted == false).FirstOrDefault();

                if (postShare != null)
                {
                    var userShare = userRepository.FindByCondition(x => x.Id == postShare.UserId).FirstOrDefault();
                    var inforUserPost = inforRepository.FindByCondition(x => x.UserId == userShare.Id).FirstOrDefault();
                    List<Image> images = imageRepository.FindByCondition(img => img.PostId == postShare.Id && img.IsDeleted == false).ToList();
                    List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == postShare.Id && vid.IsDeleted == false).ToList();
                    List<Like> likes = likeRepository.FindByCondition(img => img.PostId == postShare.Id && img.IsDeleted == false).ToList();
                    List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == postShare.Id && vid.IsDeleted == false).ToList();
                    CountLike = likes.Count();
                    CountComment = comments.Count();

                    List<Like> likesShare = likeRepository.FindByCondition(img => img.PostId == share.Id && img.IsDeleted == false).ToList();
                    List<Comment> commentsShare = commentRepository.FindByCondition(vid => vid.PostId == share.Id && vid.IsDeleted == false).ToList();
                    CountLikeShare = likes.Count();
                    CountCommentShare = comments.Count();

                    var postShareDTO = mapper.Map<PostDTO>(postShare);
                    postShareDTO.FullName = inforUserPost.FullName;
                    postShareDTO.AvatarUrl = inforUserPost.Image;
                    postShareDTO.FullNameShare = infor.FullName;
                    postShareDTO.AvatarUrlShare = infor.Image;
                    postShareDTO.Images = images;
                    postShareDTO.Videos = videos;
                    postShareDTO.Likes = likes;
                    postShareDTO.Comments = comments;
                    postShareDTO.CountLike = CountLike;
                    postShareDTO.CountComment = CountComment;
                    postShareDTO.LikesShare = likesShare;
                    postShareDTO.CommentsShare = commentsShare;
                    postShareDTO.CountLikeShare = CountLikeShare;
                    postShareDTO.CountCommentShare = CountCommentShare;
                    postShareDTO.LevelViewShare = share.LevelView;
                    postShareDTO.CreateDateShare = share.CreateDate;
                    postShareDTO.IdShare = share.Id;

                    if (like == null)
                    {
                        postShareDTO.islikeShare = false;
                    }
                    else
                    {
                        postShareDTO.islikeShare = true;
                    }
                    listPostShareDTO.Add(postShareDTO);
                }
            }
            foreach (PostDTO post in listPostShareDTO)
            {
                int has = 0;
                if (userId == _userService.UserId && post.IsDeleted == false)
                {
                    continue;
                }
                if (idOfFriends.Count != 0)
                {
                    foreach (Guid idfriend in idOfFriends)
                    {
                        if ((idfriend == userId || post.LevelViewShare == (int)(EnumLevelView.publicview)) && post.IsDeleted == false)
                        {
                            has = 1; break;
                        }
                    }
                    if (has == 0)
                    {
                        postsToRemove.Add(post);
                    }
                }
                else if (post.LevelViewShare == (int)(EnumLevelView.friendview))
                { postsToRemove.Add(post); }
            }

            foreach (var postToRemove in postsToRemove)
            {
                listPostShareDTO.Remove(postToRemove);
            }
            return listPostShareDTO;
        }
        public List<PostDTO> GetPostByUserId(Guid id)
        {
            List<Guid> idOfFriends = friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) && x.IsDeleted == false)
                .Select(x => x.UserTo == _userService.UserId ? x.UserAccept : x.UserTo)
                .ToList();
            List<Post> postsToRemove = new List<Post>();

            List<Post> entityList = postRepository.FindByCondition(x => x.UserId == id && x.IsDeleted == false).ToList() ?? throw new PostNotFoundException(id);

            List<PostDTO> dtoList = new List<PostDTO>();
            var CountLike = 0;
            var CountComment = 0;
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
                else if (post.LevelView == (int)(EnumLevelView.friendview))
                { postsToRemove.Add(post); }
            }

            foreach (var postToRemove in postsToRemove)
            {
                entityList.Remove(postToRemove);
            }
            foreach (Post entity in entityList)
            {
                var infor = inforRepository.FindByCondition(x => x.UserId == entity.UserId && x.IsDeleted == false).FirstOrDefault();
                var like = likeRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false && x.PostId == entity.Id).FirstOrDefault();
                List<Image> images = imageRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                List<Like> likes = likeRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                CountLike = likes.Count();
                CountComment = comments.Count();
                PostDTO dto = mapper.Map<PostDTO>(entity);
                dto.AvatarUrl = infor.Image;
                dto.FullName = infor.FullName;
                dto.Images = images;
                dto.Videos = videos;
                dto.Likes = likes;
                dto.Comments = comments;
                dto.CountLike = CountLike;
                dto.CountComment = CountComment;
                dto.CreateDateShare = dto.CreateDate;
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
        public List<PostDTO> GetPostsAndShareByUserId(Guid userId)
        {
            var sharedPosts = GetPostShareByUserId(userId);
            var userPosts = GetPostByUserId(userId);

            var allPosts = sharedPosts.Concat(userPosts).ToList();

            allPosts = allPosts.OrderByDescending(post => post.CreateDateShare).ToList();


            return allPosts;
        }

        public List<PostDTO> GetAllPostsAndShare()
        {
            var sharedPosts = GetAllPostShare();
            var userPosts = GetAllPost();
            var allPosts = sharedPosts.Concat(userPosts).ToList();
            allPosts = allPosts.OrderByDescending(post => post.CreateDateShare).ToList();
            return allPosts;
        }
        public void Delete(Guid id)
        {
            if (postRepository.FindById(id) == null)
                throw new PostNotFoundException(id);

            DeletePostAndRelationShip(id);

            postRepository.Save();
        }

        public List<PostDTO> GetAllPost()
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
                else if (post.LevelView == (int)(EnumLevelView.friendview))
                { postsToRemove.Add(post); }
            }

            foreach (var postToRemove in postsToRemove)
            {
                entityList.Remove(postToRemove);
            }
            foreach (Post entity in entityList)
            {
                var infor = inforRepository.FindByCondition(x => x.UserId == entity.UserId && x.IsDeleted == false).FirstOrDefault();
                var like = likeRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false && x.PostId == entity.Id).FirstOrDefault();
                List<Image> images = imageRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                List<Like> likes = likeRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
                CountLike = likes.Count();
                CountComment = comments.Count();
                PostDTO dto = mapper.Map<PostDTO>(entity);
                dto.FullName = infor.FullName;
                dto.AvatarUrl = infor.Image;
                dto.Images = images;
                dto.Videos = videos;
                dto.Likes = likes;
                dto.Comments = comments;
                dto.CountLike = CountLike;
                dto.CountComment = CountComment;
                dto.CreateDateShare = dto.CreateDate;
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
            Post post = postRepository.FindByConditionWithTracking(x => x.Id == id,  x => x.Images, x => x.Reports, x => x.TagPosts, x => x.Videos).FirstOrDefault()!;
            post.IsDeleted = true;

        }

    }
}

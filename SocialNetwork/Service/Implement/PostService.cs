using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Cloudinary;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly IImageRepository imageRepository;
        private readonly IVideoRepository videoRepository;
        private readonly ILikeRepository likeRepository;
        private readonly ICommentRepository commentRepository;
        private IGeneralService _generalService;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public PostService(IPostRepository postRepository, IUserRepository userRepository, IImageRepository imageRepository,
            IHttpContextAccessor _httpContextAccessor, SocialNetworkContext _context, Cloudinary _cloudinary,
            IVideoRepository videoRepository, ILikeRepository likeRepository, ICommentRepository commentRepository, IGeneralService generalService)
        {
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.imageRepository = imageRepository;
            this._httpContextAccessor = _httpContextAccessor;
            this._context = _context;
            this._cloudinary = _cloudinary;
            this.videoRepository = videoRepository;
            this.likeRepository = likeRepository;
            this.commentRepository = commentRepository;
            _generalService = generalService;
        }
        public string UploadFileToCloudinary(FileUploadDTO fileUploadDTO)
        {
            if (fileUploadDTO.File != null && fileUploadDTO.File.Length > 0)
            {
                if (Path.GetExtension(fileUploadDTO.File.FileName).Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                {
                    var uploadParamsVideo = new VideoUploadParams
                    {
                        File = new FileDescription(fileUploadDTO.File.FileName, fileUploadDTO.File.OpenReadStream()),
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
                if (Path.GetExtension(fileUploadDTO.File.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(fileUploadDTO.File.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(fileUploadDTO.File.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(fileUploadDTO.File.FileName).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(fileUploadDTO.File.FileName, fileUploadDTO.File.OpenReadStream()),
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
            string cloudinaryUrl = _generalService.CloudinaryUrl;
            Post post = mapper.Map<Post>(dto);
            post.UserId = _generalService.UserId;
            post.CreateBy = _generalService.UserId;
            _context.Add(post);
            _context.SaveChanges();
            postRepository.Update(post);
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
                            Link = link,
                            CreateDate = DateTime.Now,
                            CreateBy = _generalService.UserId,
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
                            CreateBy = _generalService.UserId,
                            IsDeleted = false
                        };
                        videoRepository.Create(video);
                        videoRepository.Save();
                    }
                }
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
            Post post = mapper.Map<Post>(dto);
            post.UpdateBy = _generalService.UserId;
            _context.Update(post);
            _context.SaveChanges();
            postRepository.Update(post);
            postRepository.Save();

            return dto;
        }
        public PostDTO GetById(Guid id)
        {
            Post entity = postRepository.FindById(id) ?? throw new PostNotFoundException(id);

            List<Image> images = imageRepository.FindByCondition(img => img.PostId == id).ToList();
            List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == id).ToList();
            List<Like> likes = likeRepository.FindByCondition(img => img.PostId == id).ToList();
            List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == id).ToList();
            PostDTO dto = mapper.Map<PostDTO>(entity);

            dto.Images = images;
            dto.Videos = videos;
            dto.Likes = likes;
            dto.Comments = comments;

            return dto;
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
            List<Post> entityList = postRepository.FindAll();
            List<PostDTO> dtoList = new List<PostDTO>();
            foreach (Post entity in entityList)
            {
                List<Image> images = imageRepository.FindByCondition(img => img.PostId == entity.Id).ToList();
                List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == entity.Id).ToList();
                List<Like> likes = likeRepository.FindByCondition(img => img.PostId == entity.Id).ToList();
                List<Comment> comments = commentRepository.FindByCondition(vid => vid.PostId == entity.Id).ToList();
                PostDTO dto = mapper.Map<PostDTO>(entity);
                dto.Images = images;
                dto.Videos = videos;
                dto.Likes = likes;
                dto.Comments = comments;
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

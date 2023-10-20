using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Cloudinary;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Repository;

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

        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public PostService(IPostRepository postRepository, IUserRepository userRepository, IImageRepository imageRepository,
            IHttpContextAccessor _httpContextAccessor, SocialNetworkContext _context, Cloudinary _cloudinary)
        {
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.imageRepository = imageRepository;
            this._httpContextAccessor = _httpContextAccessor;
            this._context = _context;
            this._cloudinary = _cloudinary;
        }
        public string UploadFileToCloudinary(FileUploadDTO fileUploadDTO)
        {
            if (fileUploadDTO.File != null && fileUploadDTO.File.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileUploadDTO.File.FileName, fileUploadDTO.File.OpenReadStream()),
                    Folder = "SocialNetwork/", // Đặt tên thư mục cụ thể tại đây
                };

                try
                {
                    var uploadResult = _cloudinary.Upload(uploadParams);
                    return uploadResult.SecureUrl.AbsoluteUri;
                }
                catch (Exception)
                {
                    // Xử lý lỗi tải lên, ví dụ: ghi log hoặc trả về lỗi tải lên
                    return null;
                }
            }

            return null;
        }
        public PostDTO Create(PostDTO dto, string userEmail, string cloudinaryUrl)
        {
            var user = userRepository.FindByCondition(x => x.Email == userEmail).FirstOrDefault();
            Guid id = user.Id;
            Post post = mapper.Map<Post>(dto);
            post.UserId = id;
            post.CreateBy = id;
            _context.Add(post);
            _context.SaveChanges();
            postRepository.Update(post);
            postRepository.Save();

            //Kiểm tra xem có tệp GIF được tải lên không
            if (cloudinaryUrl != null && cloudinaryUrl.Length > 0)
            {
                var link = cloudinaryUrl;
                if (link != null)
                {
                    Image image = new Image
                    {
                        PostId = post.Id,
                        Link = link,
                        CreateDate = DateTime.Now,
                        CreateBy = id,
                        IsDeleted = false
                    };
                    imageRepository.Create(image);
                    imageRepository.Save();
                }
            }
            return dto;
        }


        public PostDTO GetById(Guid id)
        {
            Post entity = postRepository.FindById(id) ?? throw new PostNotFoundException(id);

            PostDTO dto = mapper.Map<PostDTO>(entity);

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
                PostDTO dto = mapper.Map<PostDTO>(entity);
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

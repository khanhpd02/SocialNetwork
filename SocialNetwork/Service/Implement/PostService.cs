using AutoMapper;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Repository;

namespace SocialNetwork.Service.Implement
{
    public class PostService : IPostService
    {

        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public PostService(IPostRepository postRepository, IUserRepository userRepository)
        {
            this.postRepository = postRepository;
            this.userRepository = userRepository;
        }
        private void ValidateObject(PostDTO dto)
        {
            if (userRepository.FindById(dto.UserId) == null)
                throw new UserNotFoundException(dto.UserId);

        }
        public PostDTO Create(PostDTO dto)
        {
            ValidateObject(dto);

            Post Post = mapper.Map<Post>(dto);

            postRepository.Create(Post);
            postRepository.Save();

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

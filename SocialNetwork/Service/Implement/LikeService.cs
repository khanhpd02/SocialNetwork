using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Repository;

namespace SocialNetwork.Service.Implement
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository likeRepository;
        private readonly IPostRepository postRepository;
        private SocialNetworkContext _context;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public LikeService(ILikeRepository likeRepository, IPostRepository postRepository, SocialNetworkContext context)
        {
            this.likeRepository = likeRepository;
            this.postRepository = postRepository;
            _context = context;
        }

        public AppResponse LikeAndUnlike(Guid postId, Guid userId)
        {
            
            var post = postRepository.FindByCondition(x => x.Id == postId).FirstOrDefault();
            if (post == null)
            {
                return new AppResponse { message="PostId Not Valid",success=false};
            }
            else
            {
                var checklike = likeRepository.FindByCondition(x => x.UserId == userId && x.PostId==postId).FirstOrDefault();
                if (checklike == null)
                {
                    //Guid id = user.Id;
                    Like like = new Like();
                    // Post post = mapper.Map<Post>(dto);
                    like.UserId = userId;
                    like.PostId = post.Id;
                    like.CreateBy = userId;
                    like.CreateDate = DateTime.Now;
                    //like.Status = true;
                    like.IsDeleted = false;
                    _context.Add(like);
                    _context.SaveChanges();
                    likeRepository.Update(like);
                    likeRepository.Save();
                    return new AppResponse { message = "Like Sucess!", success = true };
                }
                else
                {
                    checklike.IsDeleted = !checklike.IsDeleted;
                    checklike.UpdateDate = DateTime.Now;
                    checklike.UpdateBy = userId;
                    likeRepository.Update(checklike);
                    likeRepository.Save();
                    if(checklike.IsDeleted)
                    {
                        return new AppResponse { message = "UnLike Sucess!", success = true };
                    }
                    else
                    {
                        return new AppResponse { message = "Like Sucess!", success = true };
                    }
                    
                }
            }
        }

        public AppResponse deleteLike(Guid idLike)
        {
            throw new NotImplementedException();

        }

        public AppResponse updateLike(LikeDTO likedyo, Guid userId)
        {
            throw new NotImplementedException();
        }

        public List<LikeDTO> getallByPostId(Guid postId)
        {
            List<Like> entityList = likeRepository.FindByCondition(l=>l.PostId==postId && l.IsDeleted==false);
            List<LikeDTO> dtoList = new List<LikeDTO>();
            foreach (Like entity in entityList)
            {
                LikeDTO dto = mapper.Map<LikeDTO>(entity);
                dtoList.Add(dto);
            }
            return dtoList;
           // throw new NotImplementedException();
        }

        public List<LikeDTO> getallByUserID(Guid userId)
        {
            List<Like> entityList = likeRepository.FindByCondition(l => l.UserId == userId && l.IsDeleted == false);
            List<LikeDTO> dtoList = new List<LikeDTO>();
            foreach (Like entity in entityList)
            {
                LikeDTO dto = mapper.Map<LikeDTO>(entity);
                dtoList.Add(dto);
            }
            return dtoList;
        }

       
    }
}

using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Hosting;
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
        private readonly IShareRepository shareRepository;
        private SocialNetworkContext _context;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public LikeService(ILikeRepository likeRepository, IPostRepository postRepository, 
            SocialNetworkContext context, IShareRepository shareRepository)
        {
            this.likeRepository = likeRepository;
            this.postRepository = postRepository;
            this.shareRepository = shareRepository;
            _context = context;
        }

        public AppResponse LikeAndUnlike(Guid postId, Guid userId)
        {
            var post = postRepository.FindByCondition(x => x.Id == postId).FirstOrDefault();
            var share = shareRepository.FindByCondition(x => x.Id == postId).FirstOrDefault();

            if (post == null && share != null)
            {
                var checklike = likeRepository.FindByCondition(x => x.UserId == userId && x.PostId == postId).FirstOrDefault();
                if (checklike == null)
                {
                    Like like = new Like();
                    like.UserId = userId;
                    like.PostId = share.Id;
                    likeRepository.Create(like);
                    likeRepository.Save();
                    return new AppResponse { message = "Like Success!", success = true };
                }
                else
                {
                    checklike.IsDeleted = !checklike.IsDeleted;
                    likeRepository.Update(checklike);
                    return new AppResponse { message = checklike.IsDeleted ? "Unlike Success!" : "Like Success!", success = true };
                }
            }
            else if (post != null && share == null)
            {
                var checklike = likeRepository.FindByCondition(x => x.UserId == userId && x.PostId == postId).FirstOrDefault();
                if (checklike == null)
                {
                    Like like = new Like();
                    like.UserId = userId;
                    like.PostId = post.Id;
                    likeRepository.Create(like);
                    likeRepository.Save();
                    return new AppResponse { message = "Like Success!", success = true };
                }
                else
                {
                    checklike.IsDeleted = !checklike.IsDeleted;
                    likeRepository.Update(checklike);
                    return new AppResponse { message = checklike.IsDeleted ? "Unlike Success!" : "Like Success!", success = true };
                }
            }
            else
            {
                return new AppResponse { message = "PostId Not Valid", success = false };
            }

            return new AppResponse { message = "Action Success!", success = true };
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

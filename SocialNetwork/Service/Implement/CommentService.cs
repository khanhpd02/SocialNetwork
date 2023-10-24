using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.IdentityModel.Tokens;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Repository;

namespace SocialNetwork.Service.Implement
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository postRepository;
        private SocialNetworkContext _context;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, SocialNetworkContext context)
        {
            _commentRepository = commentRepository;
            this.postRepository = postRepository;
            _context = context;
        }

        public AppResponse create(CommentDTO commentDTO, Guid userId)
        {
            if (commentDTO.Content.IsNullOrEmpty())
            {
                throw new BadRequestException("Content Không được để trống");
            }
            var post = postRepository.FindById(commentDTO.PostId);
            if(post == null)
            {
                return new AppResponse { message = "ID Post sai hoặc không tồn tại" };
            }else
            {
                Comment cmt = mapper.Map<Comment>(commentDTO);
                cmt.UserId = userId;
                cmt.CreateBy = userId;
                cmt.CreateDate = DateTime.Now;
                cmt.IsDeleted = false;
                _context.Add(cmt);
                _context.SaveChanges();
                _commentRepository.Update(cmt);
                _commentRepository.Save();
                return new AppResponse { message ="Comment Success",success = true};

            }
            

            
        }

        public AppResponse deleteOfUndo(Guid commentId, Guid userId)
        {
            var cmt = _commentRepository.FindByCondition(i=>i.Id==commentId).FirstOrDefault();
            if (cmt == null)
            {
                throw new BadRequestException("IdComment Không tồn tại");

            }
            else if (cmt.UserId == userId)
            {
                cmt.IsDeleted=!cmt.IsDeleted;
                cmt.UpdateDate = DateTime.Now;  
                cmt.UpdateBy    = userId;
                _commentRepository.Update(cmt);
                _commentRepository.Save();
                if (cmt.IsDeleted)
                {
                    return new AppResponse { message = "Xóa Cmt thành công", success = true };
                }else
                {
                    return new AppResponse { message = "Hoàn tác cmt đã xóa thành công", success = true };
                }

            }
            else
            {
                throw new BadRequestException("Không thể xóa cmt của người khác");
            }
            
        }

        public List<CommentDTO> getallofUser(Guid userId)
        {
            List<Comment> entityList = _commentRepository.FindByCondition(l => l.UserId == userId && l.IsDeleted == false);
            List<CommentDTO> dtoList = new List<CommentDTO>();
            foreach (Comment entity in entityList)
            {
                CommentDTO dto = mapper.Map<CommentDTO>(entity);
                dtoList.Add(dto);
            }
            return dtoList;
        }

        public List<CommentDTO> getallofUseronPost(Guid postId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public List<CommentDTO> getallOnPost(Guid postId)
        {
            List<Comment> entityList = _commentRepository.FindByCondition(l => l.PostId == postId && l.IsDeleted == false);
            List<CommentDTO> dtoList = new List<CommentDTO>();
            foreach (Comment entity in entityList)
            {
                CommentDTO dto = mapper.Map<CommentDTO>(entity);
                dtoList.Add(dto);
            }
            return dtoList;
        }

        public AppResponse update(CommentDTO commentDTO, Guid userID)
        {
            if (commentDTO.Content.IsNullOrEmpty())
            {
                throw new BadRequestException("Content Không được để trống");
            }
            var cmtcheck = _commentRepository.FindByCondition(cmt=>cmt.Id==commentDTO.Id).FirstOrDefault();
            if(cmtcheck == null)
            {
                throw new BadRequestException  ("ID Comment sai hoặc không tồn tại") ;
            }else if (cmtcheck.UserId==userID) 
            {
               // var cmtcheck= _commentRepository.FindById
                
                cmtcheck.Content = commentDTO.Content;
                cmtcheck.UpdateDate = DateTime.Now;
                cmtcheck.UpdateBy = userID;
                _commentRepository.Update(cmtcheck);
                _commentRepository.Save();
                return new AppResponse { message ="Update Comment Success",success = true};

            }else {
                return new AppResponse { message ="Không có quyền sửa Comment của người khác", success = false};
            }
        }
    }
}

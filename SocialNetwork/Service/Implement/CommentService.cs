using AutoMapper;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.IdentityModel.Tokens;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Repository;
using Comment = SocialNetwork.Entity.Comment;
using Video = SocialNetwork.Entity.Video;
using DocumentFormat.OpenXml.EMMA;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Service.Implement
{
    public class CommentService : ICommentService
    {
        private readonly IImageRepository imageRepository;
        private readonly IUserRepository userRepository;
        private readonly IVideoRepository videoRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IInforRepository inforRepository;
        private readonly IPostRepository postRepository;
        private readonly IShareRepository shareRepository;
        private SocialNetworkContext _context;
        private readonly Cloudinary _cloudinary;
        private IUserService _userService;
        private readonly IMasterDataRepository masterDataRepository;
        private readonly INotifyRepository notifyRepository;

        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, 
            SocialNetworkContext context, IInforRepository inforRepository,
            IShareRepository shareRepository, Cloudinary cloudinary, IUserService userService,
            IImageRepository imageRepository, IVideoRepository videoRepository, IUserRepository userRepository, 
            IMasterDataRepository masterDataRepository, 
            INotifyRepository notifyRepository)
        {
            _commentRepository = commentRepository;
            this.postRepository = postRepository;
            _context = context;
            this.inforRepository = inforRepository;
            this.shareRepository = shareRepository;
            _cloudinary = cloudinary;
            _userService = userService;
            this.imageRepository = imageRepository;
            this.videoRepository = videoRepository;
            this.userRepository = userRepository;
            this.masterDataRepository = masterDataRepository;
            this.notifyRepository = notifyRepository;
        }
        public List<string> UploadFilesToCloudinary(List<IFormFile> files)
        {
            List<string> uploadedUrls = new List<string>();
            if (files == null)
            {
                return null;
            }
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

                        var uploadResult = _cloudinary.Upload(uploadParams);
                        uploadedUrls.Add(uploadResult.SecureUrl.AbsoluteUri);

                    }
                }
            }

            return uploadedUrls;
        }
        public AppResponse create(CommentDTO commentDTO)
        {
            List<string> cloudinaryUrls = UploadFilesToCloudinary(commentDTO.File);

            if (commentDTO.Content.IsNullOrEmpty())
            {
                throw new BadRequestException("Content Không được để trống");
            }
            var post = postRepository.FindById(commentDTO.PostId);
            var share = shareRepository.FindById(commentDTO.PostId); 
            var userPost = userRepository.FindByCondition(x=>x.Id==post.UserId).FirstOrDefault().Id;
            var userShare = userRepository.FindByCondition(x => x.Id == share.UserId).FirstOrDefault().Id;
            var userPostInfor = inforRepository.FindByCondition(x => x.UserId == userPost).FirstOrDefault();
            var userShareInfor = inforRepository.FindByCondition(x => x.UserId == userShare).FirstOrDefault();
            if (post == null && share != null)
            {
                Comment cmt = mapper.Map<Comment>(commentDTO);
                cmt.PostId = share.Id;
                cmt.UserId = _userService.UserId;
                _commentRepository.Create(cmt);
                _commentRepository.Save();

                Notify notify = new Notify();
                notify.UserTo = _userService.UserId;
                notify.UserNotify = userShare;
                var notifyType = masterDataRepository.FindByCondition(x => x.Name == "Bình luận").FirstOrDefault();
                notify.Content = $"{userShareInfor.FullName} đã bình luận bài post của bạn";
                notify.NotifyType = notifyType.Id;
                notifyRepository.Create(notify);
                notifyRepository.Save();

                if (cloudinaryUrls != null)
                {
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
                                    PostId = share.Id,
                                    LinkImage = cloudinaryUrl,
                                };
                                imageRepository.Create(image);
                                imageRepository.Save();
                            }
                            else if (fileExtension.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                            {
                                var video = new Video
                                {
                                    PostId = share.Id,
                                    Link = cloudinaryUrl,
                                };
                                videoRepository.Create(video);
                                videoRepository.Save();
                            }
                        }
                    }
                }
                
                return new AppResponse { message = "Comment Success", success = true };
            }
            if (post != null && share == null)
            {
                Comment cmt = mapper.Map<Comment>(commentDTO);
                cmt.PostId = post.Id;
                cmt.UserId = _userService.UserId;
                _commentRepository.Create(cmt);
                _commentRepository.Save();

                Notify notify = new Notify();
                notify.UserTo = _userService.UserId;
                notify.UserNotify = userPost;
                var notifyType = masterDataRepository.FindByCondition(x => x.Name == "Bình luận").FirstOrDefault();
                notify.Content = $"{userPostInfor.FullName} đã bình luận bài post của bạn";
                notify.NotifyType = notifyType.Id;
                notifyRepository.Create(notify);
                notifyRepository.Save();

                if (cloudinaryUrls != null)
                {
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
                                };
                                videoRepository.Create(video);
                                videoRepository.Save();
                            }
                        }
                    }
                }
                
                return new AppResponse { message = "Comment Success", success = true };
            }
            else
            {
                return new AppResponse { message = "ID Post sai hoặc không tồn tại" };

            }
        }
        public List<CommentDTO> getAllOnPost(Guid postId)
        {
            var rootComments = _commentRepository
                .FindByCondition(l => l.PostId == postId && l.IsDeleted == false && l.ParentId == null)
                .ToList();
            List<CommentDTO> dtoList = new List<CommentDTO>();
            foreach (var rootComment in rootComments)
            {
                var comments = GetAllCommentsRecursive(rootComment.Id);
                var mappedRootComment = MapCommentDTO(rootComment);
                mappedRootComment.ChildrenComment = comments;
                dtoList.Add(mappedRootComment);
            }
            return dtoList;
        }

        public List<CommentDTO> GetAllCommentsRecursive(Guid? parentId)
        {
            List<CommentDTO> result = new List<CommentDTO>();
            var childComments = _commentRepository
                .FindByCondition(l => l.ParentId == parentId && l.IsDeleted == false)
                .ToList();
            foreach (var childComment in childComments)
            {
                var comments = GetAllCommentsRecursive(childComment.Id);
                var mappedChildComment = MapCommentDTO(childComment);
                mappedChildComment.ChildrenComment = comments;
                result.Add(mappedChildComment);
            }
            return result;
        }

        private CommentDTO MapCommentDTO(Comment comment)
        {
            var infor = inforRepository.FindByCondition(x => x.UserId == comment.UserId).FirstOrDefault();
            var mappedComment = mapper.Map<CommentDTO>(comment);
            if (infor != null)
            {
                mappedComment.FullName = infor.FullName;
                mappedComment.Image = infor.Image;
            }
            return mappedComment;
        }
        public List<CommentDTO> getallofUser(Guid userId)
        {
            var rootComments = _commentRepository
                .FindByCondition(l => l.UserId == userId && l.IsDeleted == false && l.ParentId == null)
                .ToList();
            List<CommentDTO> dtoList = new List<CommentDTO>();
            foreach (var rootComment in rootComments)
            {
                var comments = GetAllCommentsRecursive(rootComment.Id);
                var mappedRootComment = MapCommentDTO(rootComment);
                mappedRootComment.ChildrenComment = comments;
                dtoList.Add(mappedRootComment);
            }
            return dtoList;
        }
        public AppResponse deleteOfUndo(Guid commentId)
        {
            var cmt = _commentRepository.FindByCondition(i => i.Id == commentId).FirstOrDefault();
            if (cmt == null)
            {
                throw new BadRequestException("IdComment Không tồn tại");

            }
            else if (cmt.UserId == _userService.UserId)
            {
                cmt.IsDeleted = !cmt.IsDeleted;
                _commentRepository.Update(cmt);
                _commentRepository.Save();
                if (cmt.IsDeleted)
                {
                    return new AppResponse { message = "Xóa Cmt thành công", success = true };
                }
                else
                {
                    return new AppResponse { message = "Hoàn tác cmt đã xóa thành công", success = true };
                }

            }
            else
            {
                throw new BadRequestException("Không thể xóa cmt của người khác");
            }

        }

        public List<CommentDTO> getallofUseronPost(Guid postId)
        {
            throw new NotImplementedException();
        }

        public AppResponse update(CommentDTO commentDTO)
        {
            if (commentDTO.Content.IsNullOrEmpty())
            {
                throw new BadRequestException("Content Không được để trống");
            }
            var cmtcheck = _commentRepository.FindByCondition(cmt => cmt.Id == commentDTO.Id).FirstOrDefault();
            if (cmtcheck == null)
            {
                throw new BadRequestException("ID Comment sai hoặc không tồn tại");
            }
            else if (cmtcheck.UserId == _userService.UserId)
            {
                // var cmtcheck= _commentRepository.FindById

                cmtcheck.Content = commentDTO.Content;
                _commentRepository.Update(cmtcheck);
                _commentRepository.Save();
                return new AppResponse { message = "Update Comment Success", success = true };

            }
            else
            {
                return new AppResponse { message = "Không có quyền sửa Comment của người khác", success = false };
            }
        }
    }
}

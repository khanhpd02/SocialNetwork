using AngleSharp.Dom;
using AutoMapper;
using BCrypt.Net;

using Microsoft.AspNetCore.Mvc;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Repository;
using Org.BouncyCastle.Crypto.Generators;
using FirebaseAdmin.Auth.Hash;
using SocialNetwork.DTO.Response;
using DocumentFormat.OpenXml.Spreadsheet;
using SocialNetwork.Helpers;
using SocialNetwork.Repository.Implement;

namespace SocialNetwork.Service.Implement
{
    public class AdminService : IAdminService
    {
        private readonly IRoleRepository roleRepository;
        private readonly IUserRoleRepository userRoleRepository;
        private readonly IPostRepository postRepository;
        private readonly IFriendRepository friendRepository;
        private readonly IInforRepository inforRepository;
        private readonly IImageRepository imageRepository;
        private readonly IVideoRepository videoRepository;
        private readonly IRealRepository reelRepository;

        public AdminService(IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IPostRepository postRepository, IFriendRepository friendRepository, IInforRepository inforRepository, IImageRepository imageRepository, IVideoRepository videoRepository, IRealRepository reelRepository, IUserRepository userRepository)
        {
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.postRepository = postRepository;
            this.friendRepository = friendRepository;
            this.inforRepository = inforRepository;
            this.imageRepository = imageRepository;
            this.videoRepository = videoRepository;
            this.reelRepository = reelRepository;
            this.userRepository = userRepository;
        }

        private readonly IUserRepository userRepository;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();
        

        public AppResponse CreateAdmin()
        {
            
                try
                {
                    var checkAdmin = userRepository.FindByCondition(x => x.Email == "Admin@gmail.com").FirstOrDefault();
                if (checkAdmin == null)
                {
                    var getRoleAdmin = roleRepository.FindByCondition(x => x.RoleName == "Admin").FirstOrDefault();
                    userRepository.Create(new User
                    {
                        Email = "Admin@gmail.com",
                        Password = BCrypt.Net.BCrypt.EnhancedHashPassword("@admin", HashType.SHA256),
                        IsDeleted = false
                    });
                    userRoleRepository.Save();
                    var user = userRepository.FindByCondition(x => x.Email == "Admin@gmail.com").FirstOrDefault();
                    userRoleRepository.Create(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = getRoleAdmin.Id,
                        IsDeleted = false
                    });
                    userRoleRepository.Save();
                    return new AppResponse { message = "Create Admin Account Success", success = true };
                }
                else {
                    return new AppResponse { message = "Create Admin Account Fail", success = true };
                }
            }
                catch (Exception ex)
                {
                    throw new BadRequestException(ex.Message);
                }

            
        }

        public UserDTO GetUserByEmail(string Email)
        {
            var user = userRepository.FindByCondition(x => x.Email == Email).FirstOrDefault();
            if (user == null)
            {
                throw new BadRequestException("Không tìm thấy người dùng có Id: " + Email);
            }
            else
            {
                UserDTO dto = mapper.Map<UserDTO>(user);
                return dto;
            }
        }

        public UserDTO GetUserById(Guid userId)
        {
            var user = userRepository.FindByCondition(x => x.Id == userId).FirstOrDefault();
            if(user == null)
            {
                throw new BadRequestException("Không tìm thấy người dùng có Id: "+userId);
            }
            else
            {
                UserDTO dto = mapper.Map<UserDTO>(user);
                return dto;
            }
        }

        List<InforDTO> IAdminService.GetAllUser()
        {
            var admin = userRepository.FindByCondition(x => x.Email == "Admin@gmail.com").FirstOrDefault();
            List<Infor> entityList = inforRepository.FindByCondition(l => l.IsDeleted == false && l.UserId!=admin.Id);
            List<InforDTO> dtoList = new List<InforDTO>();
      
            foreach (Infor entity in entityList)
            {
                InforDTO dto = mapper.Map<InforDTO>(entity);
                var user=userRepository.FindById(dto.UserId);
                if(user.Baned==true)
                    dto.Baned= true;
                dtoList.Add(dto);
            }
            return dtoList;
        }

        public AppResponse DeleteAllUser()
        {
            var post= postRepository.FindAll().ToList();
            var friend= friendRepository.FindAll().ToList();
            var user= userRepository.FindAll().ToList() ;
            if (friend != null)
            {
                foreach (var fr in friend)
                {
                    friendRepository.Delete(fr);
                    friendRepository.Save();
                }
            }
            if (post != null)
            {
                foreach (var pos in post)
                {
                    postRepository.Delete(pos);
                    postRepository.Save();
                }
            }
            if (user != null)
            {
                foreach (var userr in user)
                {
                    userRepository.Delete(userr);
                    userRepository.Save();
                }
            }
            return new AppResponse { message = "Delete success",success=true };
        }
        public AppResponse BanUserById(Guid userId)
        {
            var user = userRepository.FindById(userId);
            if (user == null)
            {
                return new AppResponse { message = "User not found", success = false };
            }

            var userPosts = postRepository.FindByCondition(x=>x.UserId== userId).ToList();
            foreach (var post in userPosts)
            {
                post.IsDeleted = true;
                postRepository.Update(post);
                postRepository.Save();
            }

            var userFriends = friendRepository.FindByCondition(x=>x.UserTo== userId ||x.UserAccept== userId).ToList();
            foreach (var friend in userFriends)
            {
                friend.IsDeleted = true;
                friendRepository.Update(friend);
                friendRepository.Save();
            }
            var reelOfUser= reelRepository.FindByCondition(x=>x.UserId== userId).ToList();
            foreach (var reel in reelOfUser)
            {
                reel.IsDeleted = true;
                reelRepository.Update(reel);
                reelRepository.Save();
            }
            user.Baned = true;
            userRepository.Update(user);
            userRepository.Save();

            return new AppResponse { message = "Ban success", success = true };
        }

        public AppResponse DeletePostById(Guid postId)
        {
            var post = postRepository.FindByCondition(x => x.Id == postId).FirstOrDefault();
            if (post == null)
            {
                throw new BadRequestException("Không tìm thấy bài viết có Id: " + postId);
            }
            else
            {
                post.IsDeleted = true;
                postRepository.Update(post);
                postRepository.Save();
                return new AppResponse { message = "Delete success", success = true };
            }
        }
        public List<PostDTO> GetAllPosts()
        {
           
            List<Post> entityList = postRepository.FindAll();
            List<PostDTO> dtoList = new List<PostDTO>();
            var CountLike = 0;
            var CountComment = 0;
            
            foreach (Post entity in entityList)
            {
                var infor = inforRepository.FindByCondition(x => x.UserId == entity.UserId && x.IsDeleted == false).FirstOrDefault();
    
                List<Image> images = imageRepository.FindByCondition(img => img.PostId == entity.Id && img.IsDeleted == false).ToList();
                List<Video> videos = videoRepository.FindByCondition(vid => vid.PostId == entity.Id && vid.IsDeleted == false).ToList();
               
           
                PostDTO dto = mapper.Map<PostDTO>(entity);
                dto.FullName = infor.FullName;
                dto.AvatarUrl = infor.Image;
                dto.Images = images;
                dto.Videos = videos;
                dto.CountLike = CountLike;
                dto.CountComment = CountComment;
                dto.CreateDateShare = dto.CreateDate;
               

                dtoList.Add(dto);
            }

            return dtoList;
        }

        public AppResponse UnBanUserById(Guid userId)
        {
            var user = userRepository.FindByCondition(x=>x.Id == userId && x.Baned==true).FirstOrDefault();
            if (user == null)
            {
                return new AppResponse { message = "User not found or not Baned", success = false };
            }

            var userPosts = postRepository.FindByCondition(x => x.UserId == userId).ToList();
            foreach (var post in userPosts)
            {
                post.IsDeleted = false;
                postRepository.Update(post);
                postRepository.Save();
            }

            var userFriends = friendRepository.FindByCondition(x => x.UserTo == userId || x.UserAccept == userId).ToList();
            foreach (var friend in userFriends)
            {
                friend.IsDeleted = false;
                friendRepository.Update(friend);
                friendRepository.Save();
            }
            var reelOfUser = reelRepository.FindByCondition(x => x.UserId == userId).ToList();
            foreach (var reel in reelOfUser)
            {
                reel.IsDeleted = false;
                reelRepository.Update(reel);
                reelRepository.Save();
            }
            user.Baned = false;
            userRepository.Update(user);
            userRepository.Save();

            return new AppResponse { message = "Ban success", success = true };
        }
    }
}

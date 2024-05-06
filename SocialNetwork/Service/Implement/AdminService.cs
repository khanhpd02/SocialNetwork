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

namespace SocialNetwork.Service.Implement
{
    public class AdminService : IAdminService
    {
        private readonly IRoleRepository roleRepository;
        private readonly IUserRoleRepository userRoleRepository;
        private readonly IPostRepository postRepository;
        private readonly IFriendRepository friendRepository;

        public AdminService(IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IPostRepository postRepository, IFriendRepository friendRepository, IUserRepository userRepository)
        {
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.postRepository = postRepository;
            this.friendRepository = friendRepository;
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

        List<UserDTO> IAdminService.GetAllUser()
        {
            List<User> entityList = userRepository.FindByCondition(l => l.IsDeleted == false);
            List<UserDTO> dtoList = new List<UserDTO>();
            foreach (User entity in entityList)
            {
                UserDTO dto = mapper.Map<UserDTO>(entity);
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
        public AppResponse DeletePostById(Guid postId)
        {
            var post = postRepository.FindByCondition(x => x.Id == postId).FirstOrDefault();
            if (post == null)
            {
                throw new BadRequestException("Không tìm thấy bài viết có Id: " + postId);
            }
            else
            {
                postRepository.Delete(post);
                postRepository.Save();
                return new AppResponse { message = "Delete success", success = true };
            }
        }
        public List<PostDTO> GetAllPosts()
        {
            List<Post> entityList = postRepository.FindAll();
            List<PostDTO> dtoList = mapper.Map<List<PostDTO>>(entityList);
            return dtoList;
        }
    }
}

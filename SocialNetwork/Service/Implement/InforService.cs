using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DocumentFormat.OpenXml.EMMA;
using Firebase.Auth;
using Microsoft.IdentityModel.Tokens;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.FirebaseAD;
using SocialNetwork.Helpers;
using SocialNetwork.Model.User;
using SocialNetwork.Repository;
using System.Text.RegularExpressions;
using static Google.Rpc.Help.Types;

namespace SocialNetwork.Service.Implement
{
    public class InforService : IInforService
    {
        private readonly IInforRepository _inforRepository;
        private readonly IFriendRepository _friendRepository;
        private readonly IUserRepository userRepository;
        private readonly IMasterDataRepository _masterDataRepository;
        private readonly IPostRepository _postRepository;
        private readonly IRoleRepository roleRepository;
        private SocialNetworkContext _context;
        private readonly Cloudinary _cloudinary;
        private IUserService _userService;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public InforService(IInforRepository inforRepository, IFriendRepository friendRepository, IUserRepository userRepository, IMasterDataRepository masterDataRepository, IPostRepository postRepository, SocialNetworkContext context, Cloudinary cloudinary, IUserService userService)
        {
            _inforRepository = inforRepository;
            _friendRepository = friendRepository;
            this.userRepository = userRepository;
            _masterDataRepository = masterDataRepository;
            _postRepository = postRepository;
            _context = context;
            _cloudinary = cloudinary;
            _userService = userService;
        }

        async Task<InforDTO> IInforService.GetInforByUserId(Guid id)
        {
            Infor entity = _inforRepository.FindByCondition(x => x.UserId == id && x.IsDeleted == false).FirstOrDefault() ?? throw new UserNotFoundException(id);
            InforDTO dto = mapper.Map<InforDTO>(entity);
            var checkFriend = _friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId && x.UserAccept == id) || (x.UserAccept == _userService.UserId && x.UserTo == id)).FirstOrDefault();
            if (checkFriend == null)
            {
                dto.StatusFriend = "Thêm bạn bè";
            }
            else if (checkFriend.IsDeleted == true && checkFriend.UserTo == _userService.UserId)
            {
                dto.StatusFriend = "Hủy lời mời";
            }
            else if (checkFriend.IsDeleted == true && checkFriend.UserAccept == _userService.UserId)
            {
                dto.StatusFriend = "Phản Hồi";
            }
            else if (checkFriend.IsDeleted == false)
            {
                var check = _masterDataRepository.FindByCondition(x => x.Id == checkFriend.Level).FirstOrDefault();
                if (check != null)
                {
                    dto.StatusFriend = check.Name;
                }
                else
                {
                    dto.StatusFriend = "Lỗi";
                }
            }
            if (id == _userService.UserId)
            {
                dto.StatusFriend = "My Infor";
            }
            List<Guid> idOfFriends = _friendRepository.FindByCondition(x => (x.UserTo == id || x.UserAccept == id) && x.IsDeleted == false)
                .Select(x => x.UserTo == id ? x.UserAccept : x.UserTo)
                .ToList();
            var post = _postRepository.FindByCondition(x => x.UserId == id && x.IsDeleted == false).ToList();
            var user= userRepository.FindByCondition(x => x.Id==id).FirstOrDefault();
            FirebaseInitializer.InitializeFirebaseApp();
            var firebaseAuthManager = new FirebaseAuthManager();
            var firebaseResponse = await firebaseAuthManager.GetFirebaseTokenByEmailAsync(user.Email);
            if (firebaseResponse != null)
            {
                firebaseResponse.FirebaseToken = null;
                dto.FirebaseData = firebaseResponse;
                
            }
            dto.CountFriend= idOfFriends.Count();
            dto.CountPost=post.Count();

            return dto;
        }
        public List<InforDTO> GetInforByFullName(string fullname)
        {
            List<Infor> entities = _inforRepository.FindByCondition(x => x.FullName.Contains(fullname) && x.IsDeleted == false && x.UserId != _userService.UserId).ToList();

            List<InforDTO> dtos = new List<InforDTO>();

            foreach (var entity in entities)
            {
                InforDTO dto = mapper.Map<InforDTO>(entity);

                var checkFriend = _friendRepository.FindByCondition(x =>
                    (x.UserTo == _userService.UserId && x.UserAccept == entity.UserId) ||
                    (x.UserAccept == _userService.UserId && x.UserTo == entity.UserId)
                ).FirstOrDefault();

                if (checkFriend == null)
                {
                    dto.StatusFriend = "Thêm bạn bè";
                }
                else if (checkFriend.IsDeleted == true && checkFriend.UserTo == _userService.UserId)
                {
                    dto.StatusFriend = "Hủy lời mời";
                }
                else if (checkFriend.IsDeleted == true && checkFriend.UserAccept == _userService.UserId)
                {
                    dto.StatusFriend = "Phản Hồi";
                }
                else if (checkFriend.IsDeleted == false)
                {
                    var check = _masterDataRepository.FindByCondition(x => x.Id == checkFriend.Level).FirstOrDefault();
                    if (check != null)
                    {
                        dto.StatusFriend = check.Name;
                    }
                    else
                    {
                        dto.StatusFriend = "Lỗi";
                    }
                }

                if (entity.Id == _userService.UserId)
                {
                    dto.StatusFriend = "My Infor";
                }

                dtos.Add(dto);
            }
            List<Guid> idOfFriends = _friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) && x.IsDeleted == false)
                .Select(x => x.UserTo == _userService.UserId ? x.UserAccept : x.UserTo)
                .ToList();
            var userSuggest = userRepository
                .FindByCondition(x => x.IsDeleted == false && !idOfFriends.Contains(x.Id) && x.Id != _userService.UserId)
                .ToList();
            List<InforDTO> listDTOReturn = new List<InforDTO>();
            var itemsToRemove = new List<InforDTO>();
            foreach (var item in dtos)
            {
                List<Guid> idOfFriendsSuggest = _friendRepository.FindByCondition(x => (x.UserTo == item.UserId || x.UserAccept == item.UserId) && x.IsDeleted == false)
               .Select(x => x.UserTo == item.UserId ? x.UserAccept : x.UserTo)
               .ToList();
                if (idOfFriends.Intersect(idOfFriendsSuggest).Any())
                {
                    listDTOReturn.Add(item);
                    itemsToRemove.Add(item);
                }
            }
            foreach (var item in itemsToRemove)
            {
                dtos.Remove(item);
            }
            var myInfor = _inforRepository.FindByCondition(x => !x.IsDeleted && x.UserId == _userService.UserId).FirstOrDefault();
            var itemsToRemoveWards = new List<InforDTO>();
            foreach (var item in dtos)
            {
                if (item.Wards == myInfor.Wards)
                {
                    listDTOReturn.Add(item);
                    itemsToRemoveWards.Add(item);
                }
            }
            foreach (var item in itemsToRemoveWards)
            {
                dtos.Remove(item);
            }
            var itemsToRemoveDistricts = new List<InforDTO>();
            foreach (var item in dtos)
            {
                if (item.Districts == myInfor.Districts)
                {
                    listDTOReturn.Add(item);
                    itemsToRemoveDistricts.Add(item);
                }
            }
            foreach (var item in itemsToRemoveDistricts)
            {
                dtos.Remove(item);
            }
            var itemsToRemoveProvinces = new List<InforDTO>();
            foreach (var item in dtos)
            {
                if (item.Provinces == myInfor.Provinces)
                {
                    listDTOReturn.Add(item);
                    itemsToRemoveProvinces.Add(item);
                }
            }
            foreach (var item in itemsToRemoveProvinces)
            {
                dtos.Remove(item);
            }
            foreach (var item in dtos)
            {
                listDTOReturn.Add(item);
            }
            return listDTOReturn;
        }

        public AppResponse createInfo(InforDTO inforDTO, Guid userId)
        {
            if (inforDTO.FullName.IsNullOrEmpty())
            {
                throw new BadRequestException("FullName field cannot be empty");
            }
            if (!inforDTO.PhoneNumber.IsNullOrEmpty())
            {
                string sdt = inforDTO.PhoneNumber;
                if (!IsPhoneNumber(sdt))
                {
                    throw new BadRequestException("Số điện thoại không hợp lệ.");
                }
            }
            var user = userRepository.FindByCondition(x=>x.Id == userId).FirstOrDefault();
            var infor = _inforRepository.FindByCondition(x => x.UserId == userId).FirstOrDefault();
            if (infor == null)
            {
                String cloudinaryUrl = UploadFileToCloudinary(inforDTO.File);
                String cloudinaryUrlBackground = UploadFileToCloudinary(inforDTO.FileBackground);
                Infor info = mapper.Map<Infor>(inforDTO);
                info.UserId = userId;
                info.Address = inforDTO.Direction + ", " + inforDTO.Wards + ", " + inforDTO.Districts + ", " + inforDTO.Provinces;
                _inforRepository.Create(info);
                _inforRepository.Save();

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
                            info.Image = link;
                            //
                            FirebaseAuthManager authManager = new FirebaseAuthManager();
                            if (cloudinaryUrlBackground==null)
                            {
                                info.Background = "https://inkythuatso.com/uploads/thumbnails/800/2023/03/6-anh-dai-dien-trang-inkythuatso-03-15-26-36.jpg";
                            }
                            else
                            {
                                info.Background = cloudinaryUrlBackground;
                            }
                            
                            // Thông tin của người dùng mới
                            string displayName = info.FullName;
                            string email = user.Email;
                            string password = user.Password;
                            string photoUrl = link;
                            authManager.CreateUserAsync(displayName, email, password, photoUrl);

                            _inforRepository.Update(info);
                            _inforRepository.Save();



                        }
                    }

                }
                else
                {
                    info.Image = "https://res.cloudinary.com/khanhpd/image/upload/v1711113415/Avatar%20KCT/gjsek9jh65e0boofhixx.jpg";
                    _inforRepository.Update(info);
                    _inforRepository.Save();

                    FirebaseAuthManager authManager = new FirebaseAuthManager();

                    // Thông tin của người dùng mới
                    string displayName = info.FullName;
                    string email = user.Email;
                    string password = user.Password;
                    string photoUrl = "https://res.cloudinary.com/khanhpd/image/upload/v1711113415/Avatar%20KCT/gjsek9jh65e0boofhixx.jpg";
                    authManager.CreateUserAsync(displayName, email, password, photoUrl);
                }
                
                return new AppResponse { message = "Create Info Sucess!", success = true };
            }
            else
            {
                throw new BadRequestException("Người dùng đã có Infor, Không thể Create");
            }
        }

        public AppResponse deleteInfo(Guid id, Guid userId)
        {
            throw new NotImplementedException();
        }

        public AppResponse updateInfo(InforDTO inforDTO, Guid userId)
        {/*
            if (inforDTO.FullName.IsNullOrEmpty())
            {
                throw new BadRequestException("FullName field cannot be empty");
            }*/
            if (!inforDTO.PhoneNumber.IsNullOrEmpty())
            {
                string sdt = inforDTO.PhoneNumber;
                if (!IsPhoneNumber(sdt))
                {
                    throw new BadRequestException("Số điện thoại không hợp lệ.");
                }
            }
            var user=userRepository.FindByCondition(x=>x.Id==userId).FirstOrDefault();
            var infor = _inforRepository.FindByCondition(x => x.UserId == userId).FirstOrDefault();
            if (infor != null)
            {
                String cloudinaryUrl = UploadFileToCloudinary(inforDTO.File);
                String cloudinaryUrlBackground = UploadFileToCloudinary(inforDTO.FileBackground);
                //infor = mapper.Map<Infor>(inforDTO);
                if (inforDTO.FullName != null) 
                {
                    infor.FullName = inforDTO.FullName;
                    
                }
                    
                if(inforDTO.PhoneNumber != null) 
                    infor.PhoneNumber = inforDTO.PhoneNumber;
                if (inforDTO.WorkPlace != null)
                    infor.WorkPlace = inforDTO.WorkPlace;
                if (inforDTO.Gender != null)
                    infor.Gender = inforDTO.Gender;
                if (inforDTO.Address != null)
                    infor.Address = inforDTO.Address;
                if (inforDTO.DateOfBirth != null)
                    infor.DateOfBirth = inforDTO.DateOfBirth;
                if (inforDTO.Provinces != null)
                    infor.Provinces = inforDTO.Provinces;
                if (inforDTO.Districts != null)
                    infor.Districts = inforDTO.Districts;
                if (inforDTO.Wards != null)
                    infor.Wards = inforDTO.Wards;
                if (inforDTO.Direction != null)
                    infor.Direction = inforDTO.Direction;
                if (inforDTO.Career != null)
                    infor.Career = inforDTO.Career;
                if (inforDTO.Nickname != null)
                    infor.Nickname = inforDTO.Nickname;

                infor.Address = inforDTO.Direction + ", " + inforDTO.Wards + ", " + inforDTO.Districts + ", " + inforDTO.Provinces;
                _inforRepository.Update(infor);
                _inforRepository.Save();

                //
                if (cloudinaryUrl != null && cloudinaryUrl.Length > 0)
                {
                    string fileExtension = Path.GetExtension(cloudinaryUrl);
                    if (fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                           fileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                           fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                           fileExtension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
                    {
                            var link = cloudinaryUrl;                                         
                            if (inforDTO.File != null) 
                            {
                                    
                                    infor.Image = link; 
                                    
                            }
                            
                            _inforRepository.Update(infor);
                            _inforRepository.Save();
                    }

                }
                //Update image firebase
                FirebaseAuthManager authManager = new FirebaseAuthManager();
                authManager.UpdateUserAsync(infor.FullName, user.Email, infor.Image);

                if (cloudinaryUrlBackground != null && cloudinaryUrlBackground.Length > 0)
                {
                    string fileExtension = Path.GetExtension(cloudinaryUrlBackground);
                    
                        var link = cloudinaryUrlBackground;
                        

                            infor.Background = link;

                        

                        _inforRepository.Update(infor);
                        _inforRepository.Save();
                    

                }
                return new AppResponse { message = "Update Info Sucess!", success = true };


            }
            else
            {
                throw new BadRequestException("Người dùng chưa có Infor, Không thể Update");
            }

        }
        private bool IsPhoneNumber(string input)
        {
            // Biểu thức chính quy kiểm tra chuỗi có đúng 10 chữ số hay không
            string pattern = @"^\d{10}$";

            Regex regex = new Regex(pattern);//true= số điện thoại hợp lệ

            return regex.IsMatch(input);

        }
        public string UploadFileToCloudinary(IFormFile fileUploadDTO)
        {
            if (fileUploadDTO != null && fileUploadDTO.Length > 0)
            {

                if (Path.GetExtension(fileUploadDTO.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(fileUploadDTO.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(fileUploadDTO.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(fileUploadDTO.FileName).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(fileUploadDTO.FileName, fileUploadDTO.OpenReadStream()),
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

        async Task<InforDTO> IInforService.GetMyInfor()
        {
            Infor entity = _inforRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false).FirstOrDefault() ?? throw new UserNotFoundException(_userService.UserId);
            InforDTO dto = mapper.Map<InforDTO>(entity);
            List<UserRole> userRole = _context.UserRoles.Where(u => u.UserId == _userService.UserId).ToList();
            List<string> roles = new List<string>();
            foreach (var UserRole in userRole)
            {
                var role = _context.Roles.Where(u => u.Id == UserRole.RoleId).FirstOrDefault();
                roles.Add(role.RoleName.ToString());
            }
            dto.RoleName = roles;
            FirebaseInitializer.InitializeFirebaseApp();
            List<Guid> idOfFriends = _friendRepository.FindByCondition(x => (x.UserTo == _userService.UserId || x.UserAccept == _userService.UserId) && x.IsDeleted == false)
                .Select(x => x.UserTo == _userService.UserId ? x.UserAccept : x.UserTo)
                .ToList();
            var mypost=_postRepository.FindByCondition(x=>x.UserId==_userService.UserId&&x.IsDeleted==false).ToList();
            var firebaseAuthManager = new FirebaseAuthManager();
            var firebaseResponse = await firebaseAuthManager.GetFirebaseTokenByEmailAsync(_userService.UserEmail);
            dto.CountFriend=idOfFriends.Count();
            dto.CountPost=mypost.Count();
            if (firebaseResponse != null)
            {
                dto.FirebaseData = firebaseResponse;
            }
            return dto;

            
        }
    }
}

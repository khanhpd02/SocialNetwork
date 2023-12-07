﻿using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.IdentityModel.Tokens;
using Service.Implement.ObjectMapping;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Repository;
using System.Text.RegularExpressions;

namespace SocialNetwork.Service.Implement
{
    public class InforService : IInforService
    {
        //private readonly IInforService _inforService;
        private readonly IInforRepository _inforRepository;
        private readonly IFriendRepository _friendRepository;
        private readonly IMasterDataRepository _masterDataRepository;
        private SocialNetworkContext _context;
        private readonly Cloudinary _cloudinary;
        private IUserService _userService;
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        public InforService(IInforRepository inforRepository, SocialNetworkContext context, Cloudinary cloudinary)
        {
            _inforRepository = inforRepository;
            _context = context;
            _cloudinary = cloudinary;
        }

        public InforService(IInforRepository inforRepository, IFriendRepository friendRepository, IMasterDataRepository masterDataRepository, SocialNetworkContext context, Cloudinary cloudinary)
        {
            _inforRepository = inforRepository;
            _friendRepository = friendRepository;
            _masterDataRepository = masterDataRepository;
            _context = context;
            _cloudinary = cloudinary;
        }

        public InforService(IInforRepository inforRepository, IFriendRepository friendRepository, IMasterDataRepository masterDataRepository, SocialNetworkContext context, Cloudinary cloudinary, IUserService userService)
        {
            _inforRepository = inforRepository;
            _friendRepository = friendRepository;
            _masterDataRepository = masterDataRepository;
            _context = context;
            _cloudinary = cloudinary;
            _userService = userService;
        }
        public InforDTO GetMyInfor()
        {
            Infor entity = _inforRepository.FindByCondition(x => x.UserId == _userService.UserId && x.IsDeleted == false).FirstOrDefault() ?? throw new UserNotFoundException(_userService.UserId);
            InforDTO dto = mapper.Map<InforDTO>(entity);
            return dto;
        }
        public InforDTO GetInforByUserId(Guid id)
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

            return dto;
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

            var infor = _inforRepository.FindByCondition(x => x.UserId == userId).FirstOrDefault();
            if (infor == null)
            {
                String cloudinaryUrl = UploadFileToCloudinary(inforDTO.File);
                Infor info = mapper.Map<Infor>(inforDTO);
                info.UserId = userId;
                //info.UpdateDate = DateTime.Now;
                info.CreateDate = DateTime.Now;
                info.CreateBy = userId;
                //info.UpdateBy = userId;
                info.IsDeleted = false;
                _context.Add(info);
                _context.SaveChanges();

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
                        if (link != null)
                        {
                            info.Image = link;
                            _inforRepository.Update(info);
                            _inforRepository.Save();



                        }
                    }

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

            var infor = _inforRepository.FindByCondition(x => x.UserId == userId).FirstOrDefault();
            if (infor != null)
            {
                String cloudinaryUrl = UploadFileToCloudinary(inforDTO.File);
                //infor = mapper.Map<Infor>(inforDTO);
                infor.FullName = inforDTO.FullName;
                infor.PhoneNumber = inforDTO.PhoneNumber;
                infor.WorkPlace = inforDTO.WorkPlace;
                infor.Gender = inforDTO.Gender;
                infor.Address = inforDTO.Address;
                infor.DateOfBirth = inforDTO.DateOfBirth;
                infor.UserId = userId;
                infor.UpdateDate = DateTime.Now;
                infor.UpdateBy = userId;
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
                        if (link != null)
                        {
                            infor.Image = link;
                            _inforRepository.Update(infor);
                            _inforRepository.Save();

                        }
                    }

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
    }
}

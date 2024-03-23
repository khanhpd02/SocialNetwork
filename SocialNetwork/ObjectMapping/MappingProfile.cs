using AutoMapper;
using Newtonsoft.Json.Linq;
using SocialNetwork.DTO;
using SocialNetwork.Entity;
using SocialNetwork.Model.User;

namespace Service.Implement.ObjectMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            //CreateMap<TemplateCard, TemplateCardDTO>()
            //    .ForMember(dest => dest.CategoryIdList, opt => opt.MapFrom(src => src.Categories.Select(c => c.Id).ToList()))
            //    .ForMember(dest => dest.JsonData, opt =>
            //    {
            //        opt.PreCondition(src => IsValidJson(src.Data!));
            //        opt.MapFrom(src => JObject.Parse(src.Data!));
            //    })
            //    .ForMember(dest => dest.TagIdList, opt => opt.MapFrom(src => src.Tags.Select(t => t.Id).ToList()));
            CreateMap<Comment, CommentDTO>();
            CreateMap<Friend, FriendDTO>();
            CreateMap<Group, GroupDTO>();
            CreateMap<GroupChat, GroupChatDTO>();
            CreateMap<Image, ImageDTO>();
            CreateMap<Infor, InforDTO>();
            CreateMap<Like, LikeDTO>();
            CreateMap<PinCode, PinCodeDTO>();
            CreateMap<Post, PostDTO>();
            CreateMap<Report, ReportDTO>();
            CreateMap<Role, RoleDTO>();
            CreateMap<Tag, TagDTO>();
            CreateMap<TagPost, TagPostDTO>();
            CreateMap<User, UserDTO>();
            CreateMap<User, RegisterModel>();
            CreateMap<UserRole, UserRoleDTO>();
            CreateMap<Video, VideoDTO>();
            CreateMap<Notify, NotifyDTO>();
            CreateMap<Share, ShareDTO>();
            CreateMap<Audio, AudioDTO>();
            CreateMap<Real, RealDTO>();
            CreateMap<MasterDatum, MasterDatumDTO>();

            CreateMap<User, UserDTO>().ReverseMap();


            CreateMap<CommentDTO, Comment>();
            CreateMap<FriendDTO, Friend>();
            CreateMap<GroupDTO, Group>();
            CreateMap<GroupChatDTO, GroupChat>();
            CreateMap<ImageDTO, Image>();
            CreateMap<InforDTO, Infor>();
            CreateMap<LikeDTO, Like>();
            CreateMap<PinCodeDTO, PinCode>();
            CreateMap<PostDTO, Post>();
            CreateMap<ReportDTO, Report>();
            CreateMap<RoleDTO, Role>();
            CreateMap<TagDTO, Tag>();
            CreateMap<TagPostDTO, TagPost>();
            CreateMap<UserDTO, User>();
            CreateMap<UserRoleDTO, UserRole>();
            CreateMap<VideoDTO, Video>();
            CreateMap<RegisterModel, User>();
            CreateMap<NotifyDTO, Notify>();
            CreateMap<ShareDTO, Share>();
            CreateMap<AudioDTO, Audio>();
            CreateMap<RealDTO, Real>();
            CreateMap<MasterDatumDTO, MasterDatum>();



        }

        private bool IsValidJson(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue)) { return false; }
            try
            {
                var obj = JObject.Parse(stringValue);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

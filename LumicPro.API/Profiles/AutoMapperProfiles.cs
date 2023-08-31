using AutoMapper;
using LumicPro.Application.Models;
using LumicPro.Core.Entities;

namespace LumicPro.Application.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AddUserDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(x => x.Email))
                .ReverseMap();
            CreateMap<AppUser, UserResponse>().ReverseMap();
           // CreateMap<AppUser, UserResponse>().ReverseMap();   
        }
    }
}

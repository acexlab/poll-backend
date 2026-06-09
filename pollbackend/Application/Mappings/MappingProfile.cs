using AutoMapper;
using pollbackend.Application.DTOs;
using pollbackend.Domain.Entities;
using System.Linq;

namespace pollbackend.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            
            CreateMap<User, UserListDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<PollOption, PollOptionDto>();

            CreateMap<Poll, PollDto>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
        }
    }
}

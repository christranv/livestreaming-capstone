using AutoMapper;
using Identity.API.Models;
using Identity.API.Models.DTO;

namespace Identity.API.Configuration
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, ApplicationUserDto>()
                    .ForMember(d
                        => d.FollowingCount, otp
                        => otp.MapFrom(src
                            => src.FollowingUsers.Count))
                    .ForMember(d
                        => d.FollowedCount, otp
                        => otp.MapFrom(src
                            => src.FollowedByUsers.Count));
                cfg.CreateMap<ApplicationUser, ApplicationUserSearchDto>()
                    .ForMember(d
                        => d.FollowedCount, otp
                        => otp.MapFrom(src
                            => src.FollowedByUsers.Count));
            });
        }
    }
}
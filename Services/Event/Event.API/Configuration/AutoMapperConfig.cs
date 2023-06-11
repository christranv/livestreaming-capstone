using AutoMapper;
using Event.API.Models;
using Event.API.ViewModel;

namespace Event.API.Configuration
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Models.Event, EventViewModel>();
                cfg.CreateMap<SubEvent, SubEventViewModel>()
                    .ForMember(des => des.Name,
                        act => act.MapFrom(src => src.Event.Name));
                cfg.CreateMap<SubEventFollower, SubEventFollowerViewModel>();
            });
        }
    }
}
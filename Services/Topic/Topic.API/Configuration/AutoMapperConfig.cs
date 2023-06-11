using System.Linq;
using AutoMapper;
using Topic.API.Models;
using Topic.API.ViewModel;

namespace Topic.API.Configuration
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TopicTag, TagViewModel>();
                cfg.CreateMap<TopicCategory, CategoryViewModel>()
                    .ForMember(d 
                        => d.Tags, otp 
                        => otp.MapFrom(src 
                        => src.CategoryTags.Select(ct => ct.Tag)
                        .ToList()));
                cfg.CreateMap<TopicTag, CategoryViewModel>()
                    .ForMember(d => d.Tags, 
                        otp
                            => otp.MapFrom(src 
                                => src.CategoryTags.Select(ct => ct.Tag)
                        .ToList()));
            });
        }
    }
}

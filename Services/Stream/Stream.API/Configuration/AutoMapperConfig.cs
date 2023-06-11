using AutoMapper;
using Microsoft.Extensions.Configuration;
using Stream.API.Application.Models;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;

namespace Stream.API.Configuration
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration Configure(IConfiguration configuration)
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TagDTO, StreamSessionTag>();
                cfg.CreateMap<StreamSessionTag, TagDTO>()
                    .ForMember(o => o.Id,
                        otp => otp.MapFrom(ss => ss.TagGuid));
                cfg.CreateMap<StreamSessionCategory, CategoryDTO>()
                    .ForMember(o => o.Id,
                        otp => otp.MapFrom(ss => ss.CategoryGuid));
                cfg.CreateMap<StreamSession, StreamSessionDTO>()
                    .ForMember(o => o.ThumbnailImage,
                        otp => otp.MapFrom(ss => $"{configuration["SrsApiServerUrl"]}/{ss.Streamer.StreamKey}-001.png"))
                    .ForMember(o => o.StreamUrlSource,
                        otp => otp.MapFrom(ss =>
                            $"{configuration["SrsApiServerUrl"]}/{ss.Streamer.StreamKey}/source.m3u8"))
                    .ForMember(o => o.StreamUrl480P,
                        otp => otp.MapFrom(
                            ss => $"{configuration["SrsApiServerUrl"]}/{ss.Streamer.StreamKey}/480p.m3u8"))
                    .ForMember(o => o.StreamUrl720P,
                        otp => otp.MapFrom(
                            ss => $"{configuration["SrsApiServerUrl"]}/{ss.Streamer.StreamKey}/720p.m3u8"))
                    .ForMember(o => o.Category,
                        otp => otp.MapFrom(ss => ss.Category))
                    .ForMember(o => o.Tags,
                        otp => otp.MapFrom(ss => ss.Tags))
                    .ForMember(o => o.StreamerIdentityGuid,
                        otp => otp.MapFrom(ss => ss.Streamer.IdentityGuid));
                cfg.CreateMap<Streamer, StreamerMediaSourceDTO>()
                    .ForMember(o => o.StreamUrlSource,
                        otp => otp.MapFrom(s =>
                            $"{configuration["SrsApiServerUrl"]}/{s.StreamKey}/source.m3u8"))
                    .ForMember(o => o.StreamUrl480P,
                        otp => otp.MapFrom(
                            s => $"{configuration["SrsApiServerUrl"]}/{s.StreamKey}/480p.m3u8"))
                    .ForMember(o => o.StreamUrl720P,
                        otp => otp.MapFrom(
                            s => $"{configuration["SrsApiServerUrl"]}/{s.StreamKey}/720p.m3u8"));
                cfg.CreateMap<StreamSession, SearchStreamSessionDTO>()
                    .ForMember(o => o.ThumbnailImage,
                        otp => otp.MapFrom(ss => $"{configuration["SrsApiServerUrl"]}/{ss.Streamer.StreamKey}-001.png"))
                    .ForMember(o => o.Tags,
                        otp => otp.MapFrom(ss => ss.Tags))
                    .ForMember(o => o.StreamerIdentityGuid,
                        otp => otp.MapFrom(ss => ss.Streamer.IdentityGuid));
                cfg.CreateMap<StreamSession, StreamStatusDTO>()
                    .ForMember(o => o.Category,
                        otp => otp.MapFrom(ss => ss.Category))
                    .ForMember(o => o.StreamerIdentityGuid,
                        otp => otp.MapFrom(ss => ss.Streamer.IdentityGuid));
            });
        }
    }
}
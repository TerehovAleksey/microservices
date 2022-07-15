using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<CommandCreateDto, Command>();
        CreateMap<Command, CommandReadDto>();
        CreateMap<PlatformPublishedDto, Platform>()
            .ForMember(d => d.ExternalId, opt => opt.MapFrom(t => t.Id));
        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(d => d.ExternalId, opt => opt.MapFrom(t => t.PlatformId))
            .ForMember(d => d.Name, opt => opt.MapFrom(t => t.Name))
            .ForMember(d => d.Commands, opt => opt.Ignore());
    }
}
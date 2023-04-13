using AutoMapper;
using Innoloft.Domain.Entities;
using Innoloft.Web.Controllers.Dtos;

namespace Innoloft.Web.Helpers.MappingProfiles;

public class EventMappingProfile : Profile
{
    public EventMappingProfile()
    {
        CreateMap<CreateEventDto, Event>();
        CreateMap<UpdateEventDto, Event>();
    }
}
using AutoMapper;
using Domain;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // AutoMapper will check the properties inside the Project class (DB)
        // It will match the property names coming from the request
        CreateMap<Project, Project>();
    }
}
using Application.Comments;
using Application.Photos;
using Application.Projects;
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
        CreateMap<Project, ProjectDto>()
            .ForMember(destination => destination.Owner, 
                option => option.MapFrom(
                    source => source.Owner));
        CreateMap<User, Profiles.Profile>()
            // .ForMember(destination => destination.Id, option => option.MapFrom(source => source.Id))
            .ForMember(destination => destination.UserName, option => option.MapFrom(source => source.UserName))
            .ForMember(destination => destination.FirstName, option => option.MapFrom(source => source.FirstName))
            .ForMember(destination => destination.Email, option => option.MapFrom(source => source.Email))
            .ForMember(destination => destination.PhoneNumber, option => option.MapFrom(source => source.PhoneNumber))
            .ForMember(destination => destination.Bio, option => option.MapFrom(source => source.Bio));

        CreateMap<Photo, PhotoDto>();
        // .ForMember()
        
        CreateMap<Comment, CommentDto>()
            .ForMember(destination => destination.FirstName, option => option.MapFrom(source => source.Author.FirstName))
            .ForMember(destination => destination.Body, option => option.MapFrom(source => source.Body))
            .ForMember(destination => destination.CreatedAt, option => option.MapFrom(source => source.CreatedAt));
    }
}
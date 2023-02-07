using Application.Profiles;
using Domain;

namespace Application.Projects;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Attachment { get; set; }
    public string Client { get; set; }
    
    public ProfileDto Owner { get; set; }
}
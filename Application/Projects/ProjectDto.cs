using Application.Comments;
using Application.Photos;
using Application.Profiles;

namespace Application.Projects;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public string Description { get; set; }
    public ICollection<PhotoDto> Photos { get; set; }
    public ICollection<CommentDto> Comments { get; set; }
    public ProfileDto Owner { get; set; }
}
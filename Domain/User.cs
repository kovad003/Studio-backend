using Microsoft.AspNetCore.Identity;

namespace Domain;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public string Company { get; set; }
    public string Avatar { get; set; }
    
    // One-to-many rel:
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Photo> Photos { get; set; }
}
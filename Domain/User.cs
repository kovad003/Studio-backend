using Microsoft.AspNetCore.Identity;

namespace Domain;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public string Company { get; set; }
}
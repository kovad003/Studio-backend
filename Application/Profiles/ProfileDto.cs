namespace Application.Profiles;

public class ProfileDto
{
    // Automapper.ProjectTo gives an error to Guid type!
    // If string is not ok for the Id than it must be mapped manually
    // public Guid Id { get; set; }
    public string Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Bio { get; set; }
}
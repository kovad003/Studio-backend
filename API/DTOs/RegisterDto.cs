using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    // [Required]
    // public string UserName { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public string Bio { get; set; }
    [Required]
    public string Company { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
}
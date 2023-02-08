using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class PasswordDto
{
    public string Id { get; set; }
    [Required]
    public string NewPassword { get; set; }
    public string CurrentPassword { get; set; }
}
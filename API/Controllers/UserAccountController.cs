using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserAccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly TokenService _tokenService;

    public UserAccountController(UserManager<User> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized();

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (result)
        {
            return new UserDto()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                Company = user.Company,
                Token = _tokenService.CreateToken(user),
                Avatar = "placeholder: Avatar",
                Bio = user.Bio,
            };
        }

        return Unauthorized();
    }
}
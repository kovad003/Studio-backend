using System.Security.Claims;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized();

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (result)
        {
            return await CreateUserDto(user);
        }

        return Unauthorized();
    }

    // [AllowAnonymous]
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        // if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName))
        // {
        //     return BadRequest("Username is already taken");
        // }

        var user = new User
        {
            UserName = registerDto.UserName,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Bio = registerDto.Bio,
            Company = registerDto.Company,
            Avatar = registerDto.Avatar,
            PhoneNumber = registerDto.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        var result2 = await _userManager.AddToRoleAsync(user, "Client");
        
        if (result.Succeeded)
        {
            return await CreateUserDto(user);
        }

        return BadRequest(result.Errors);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
        return await CreateUserDto(user);
    }
    

    private async Task<ActionResult<UserDto>> CreateUserDto(User user)
    {
        return new UserDto()
        {
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Bio = user.Bio,
            Company = user.Company,
            Avatar = user.Avatar,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Token = await _tokenService.CreateToken(user),
        };
    }
}
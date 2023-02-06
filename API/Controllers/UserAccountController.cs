using System.Security.Claims;
using API.DTOs;
using API.Services;
using API.Tools;
using Domain;
using Microsoft.AspNetCore.Authorization;
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

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized();

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (result)
        {
            return await CreateUserDto(user, true);
        }

        return Unauthorized();
    }

    // [AllowAnonymous]
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        string userName;
        int number = 0;
        do
        {
            number++;
            userName = UsernameGenerator.Generate(registerDto, number);
            
            var u = await _userManager.FindByNameAsync(userName);
            if (u != null)
                userName = null;

        } while (userName == null);
        
        var user = new User
        {
            // UserName = registerDto.UserName,
            UserName = userName,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            // Bio = registerDto.Bio,
            Company = registerDto.Company,
            PhoneNumber = registerDto.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
            result = await _userManager.AddToRoleAsync(user, registerDto.Role);
           
        if (result.Succeeded)
            return await CreateUserDto(user, false);
       
        return BadRequest(result.Errors);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
        return await CreateUserDto(user, true);
    }

    private async Task<ActionResult<UserDto>> CreateUserDto(User user, bool hasToken)
    {
        var userRole = await _userManager.GetRolesAsync(user);
        string token = null;

        if (hasToken)
            token = await _tokenService.CreateToken(user);
        
        return new UserDto()
        {
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Bio = user.Bio,
            Company = user.Company,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            // Token = await _tokenService.CreateToken(user),
            Token = token,
            Role = userRole.FirstOrDefault(),
        };
    }
}
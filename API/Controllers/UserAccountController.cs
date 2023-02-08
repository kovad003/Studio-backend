using System.Security.Claims;
using API.DTOs;
using API.Services;
using API.Tools;
using Application.Profiles;
using Domain;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProfileDto = API.DTOs.ProfileDto;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserAccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly TokenService _tokenService;
    private UserAccessor _userAccessor;

    public UserAccountController(UserManager<User> userManager, TokenService tokenService, UserAccessor userAccessor)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _userAccessor = userAccessor;
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
    [HttpPost("register-client")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (registerDto == null)
            return BadRequest("Username cannot be generated!");
        
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
    
    [Authorize(Roles = "Client")]
    [HttpPut("update-profile")]
    // [Route("UpdateProfile")]
    public async Task<ActionResult<UserDto>> UpdateProfile(ProfileDto dto)
    {
        var id = _userAccessor.GetUserId();
        
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return BadRequest("User not found in DB");

        // Updating user object:
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Bio = dto.Bio;
        user.PhoneNumber = dto.PhoneNumber;
        user.Email = dto.Email;

        // Updating DB
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return await CreateUserDto(user, false);
        return BadRequest(result.Errors);
    }
    
    [Authorize(Roles = "Client")]
    [HttpPut("change-password")]
    // [Route("UpdateProfile")]
    public async Task<ActionResult<UserDto>> ChangePassword(PasswordDto dto)
    {
        var id = _userAccessor.GetUserId();
        
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return BadRequest("User not found in DB");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        
        if (result.Succeeded)
            return await CreateUserDto(user, true);
        return BadRequest(result.Errors);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        // var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
        var id = _userAccessor.GetUserId();
        var user = await _userManager.FindByIdAsync(id);
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
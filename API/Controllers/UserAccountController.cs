using API.DTOs;
using API.Services;
using API.Tools;
using Application.Interfaces;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using ProfileDto = API.DTOs.ProfileDto;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserAccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly TokenService _tokenService;
    private readonly UserAccessor _userAccessor;
    private readonly DataContext _dataContext;
    private readonly IPhotoAccessor _photoAccessor;

    public UserAccountController(UserManager<User> userManager, 
        TokenService tokenService, UserAccessor userAccessor, DataContext dataContext, IPhotoAccessor photoAccessor, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _userAccessor = userAccessor;
        _dataContext = dataContext;
        _photoAccessor = photoAccessor;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            // await _userManager.AccessFailedAsync(null);
            return Unauthorized();
        }

        // var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
            return await CreateUserDto(user, true);
        }
        if (result.IsLockedOut)
        {
            // Handle locked-out user
            return BadRequest($"This account is locked out for {user.LockoutEnd.Value.UtcDateTime:yyyy-MM-dd HH:mm:ss} UTC.");
        }
        // await _userManager.AccessFailedAsync(user);

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

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete-profile/{id}")]
    public async Task<ActionResult> DeleteProfile(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();
        
        var photos = _dataContext.Photos
            .Include(p => p.Project)
            .Where(p => p.Project.Owner.Id == id).ToList();

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            foreach (var photo in photos)
                await _photoAccessor.DeletePhoto(photo.Id);
            return Ok("User deleted successfully");
        }

        return BadRequest(result.Errors);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-users")]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var dto = await CreateUserDto(user, false);
            userDtos.Add(dto.Value);
        }
        
        return Ok(userDtos);
    }

    [Authorize(Roles = "Client")]
    [HttpPut("update-profile")]
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
            Id = user.Id,
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
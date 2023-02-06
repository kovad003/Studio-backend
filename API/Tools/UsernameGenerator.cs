using API.DTOs;

namespace API.Tools;

public static class UsernameGenerator
{
    public static string Generate(RegisterDto registerDto, int number)
    {
        // var username = registerDto.Email.Split('@')[0];
        var username = registerDto.LastName
            + number;
        
        return username;
    }
}
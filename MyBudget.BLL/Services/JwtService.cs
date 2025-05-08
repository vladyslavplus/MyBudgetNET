using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyBudget.BLL.DTOs.Auth;
using MyBudget.BLL.Services.Interfaces;
using MyBudget.DAL.Entities;

namespace MyBudget.BLL.Services;

public class JwtService : IJwtService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public JwtService(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        var user = await _userManager.FindByNameAsync(loginRequestDto.UserName!);
        if (user is null || !await _userManager.CheckPasswordAsync(user, loginRequestDto.Password!))
            throw new UnauthorizedAccessException("Invalid username or password");

        var key = Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]!);

        var validityMinutes = int.TryParse(_configuration["JwtConfig:TokenValidityMins"], out var mins)
            ? mins
            : 30;

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(validityMinutes),
            Issuer = _configuration["JwtConfig:Issuer"],
            Audience = _configuration["JwtConfig:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginResponseDto
        {
            UserName = user.UserName,
            Email = user.Email,
            AccessToken = tokenHandler.WriteToken(token),
            ExpiresIn = validityMinutes * 60
        };
    }

    public async Task<bool> RegisterAsync(string username, string email, string password)
    {
        if (await _userManager.FindByNameAsync(username) is not null ||
            await _userManager.FindByEmailAsync(email) is not null)
            return false;

        var user = new User
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await _userManager.AddToRoleAsync(user, "User");

        return result.Succeeded;
    }
}
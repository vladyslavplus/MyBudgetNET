using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyBudget.BLL.DTOs.Auth;
using MyBudget.BLL.Exceptions;
using MyBudget.BLL.Services.Interfaces;
using MyBudget.DAL.Entities;

namespace MyBudget.BLL.Services;

public class JwtService : IJwtService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;

    public JwtService(UserManager<User> userManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        var user = await _userManager.FindByNameAsync(loginRequestDto.UserName!);
        if (user is null || !await _userManager.CheckPasswordAsync(user, loginRequestDto.Password!))
            throw new JwtUnauthorizedException("Invalid username or password");

        if(user.IsBlocked)
            throw new JwtUnauthorizedException("User is blocked");
        
        if (!user.EmailConfirmed)
            throw new JwtUnauthorizedException("Email is not confirmed.");
        
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = GenerateJwtToken(user, roles);
        var expiresIn = GetTokenValiditySeconds();

        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var refreshToken = GenerateRefreshToken(ipAddress);
        user.RefreshTokens.Add(refreshToken);

        CleanOldRefreshTokens(user);
        await _userManager.UpdateAsync(user);

        SetRefreshTokenCookie(refreshToken.Token, refreshToken.Expires);

        return new LoginResponseDto
        {
            UserName = user.UserName,
            Email = user.Email,
            AccessToken = accessToken,
            ExpiresIn = GetTokenValiditySeconds()
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
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) return false;

        await _userManager.AddToRoleAsync(user, "User");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        var apiBaseUrl = _configuration["ApiBaseUrl"]; 
        var confirmationLink = $"{apiBaseUrl}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

        await _emailService.SendEmailAsync(
            email, 
            "Confirm your email", 
            confirmationLink
        );

        return true;
    }

    
    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string ipAddress)
    {
        var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            throw new JwtTokenMissingException();

        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user is null)
            throw new JwtTokenInvalidException();
        
        if(user.IsBlocked)
            throw new JwtUnauthorizedException("User is blocked");

        var token = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken)
                    ?? throw new JwtTokenInvalidException();

        if (!token.IsActive)
            throw new JwtTokenExpiredException();

        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;

        var newRefreshToken = GenerateRefreshToken(ipAddress);
        token.ReplacedByToken = newRefreshToken.Token;
        user.RefreshTokens.Add(newRefreshToken);

        CleanOldRefreshTokens(user);
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = GenerateJwtToken(user, roles);

        SetRefreshTokenCookie(newRefreshToken.Token, newRefreshToken.Expires);

        return new RefreshTokenResponseDto
        {
            AccessToken = accessToken,
            ExpiresIn = GetTokenValiditySeconds()
        };
    }
    
    public async Task<bool> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId)
                   ?? throw new JwtUnauthorizedException("User not found");

        if(user.IsBlocked)
            throw new JwtUnauthorizedException("User is blocked");
        
        var decoded = Uri.UnescapeDataString(token);
        var result = await _userManager.ConfirmEmailAsync(user, decoded);
        return result.Succeeded;
    }
    
    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return false;

        if(user.IsBlocked)
            throw new JwtUnauthorizedException("User is blocked");
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var encodedToken = Uri.EscapeDataString(token);

        var message = $"Your password reset code is:\n\n{encodedToken}\n\n" +
                      "Use this code along with your email to reset your password.";

        await _emailService.SendEmailAsync(email, "Reset Your Password Code", message);
        return true;
    }
    
    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return false;

        var decodedToken = Uri.UnescapeDataString(token);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);
        return result.Succeeded;
    }
    
    public async Task LogoutAsync()
    {
        var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            throw new JwtTokenMissingException();
        
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user is null)
            throw new JwtTokenInvalidException();
        
        var token = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken);
        if (token is not { IsActive: true })
            throw new JwtTokenExpiredException();

        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        await _userManager.UpdateAsync(user);

        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("refreshToken");
    }
    
    private void CleanOldRefreshTokens(User user, int maxActiveTokens = 5)
    {
        var activeTokens = user.RefreshTokens
            .Where(t => t.IsActive)
            .OrderByDescending(t => t.Created)
            .ToList();

        if (activeTokens.Count <= maxActiveTokens)
            return;

        var tokensToRemove = activeTokens.Skip(maxActiveTokens).ToList();
        foreach (var token in tokensToRemove)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = "auto-cleanup";
        }
    }
    
    private string GenerateJwtToken(User user, IList<string> roles)
    {
        var claims = GenerateClaims(user, roles);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(GetTokenValidityMinutes()),
            Issuer = _configuration["JwtConfig:Issuer"],
            Audience = _configuration["JwtConfig:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]!)),
                SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static IEnumerable<Claim> GenerateClaims(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }
    
    private static RefreshToken GenerateRefreshToken(string ipAddress)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }

    private void SetRefreshTokenCookie(string token, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,   
            Secure = true,
            SameSite = SameSiteMode.Strict, 
            Expires = expires
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
    
    private int GetTokenValidityMinutes()
        => int.TryParse(_configuration["JwtConfig:TokenValidityMins"], out var mins) ? mins : 30;

    private int GetTokenValiditySeconds()
        => GetTokenValidityMinutes() * 60;
}
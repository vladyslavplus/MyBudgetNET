using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBudget.BLL.DTOs.Auth;
using MyBudget.BLL.Services.Interfaces;

namespace MyBudget.JWT.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;

    public AuthController(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
    {
        var loginModel = await _jwtService.Login(loginDto);
        var responseDto = loginModel.Adapt<LoginResponseDto>();
        return Ok(responseDto);
    }
    
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
    {
        var success = await _jwtService.RegisterAsync(
            registerDto.UserName!, 
            registerDto.Email!, 
            registerDto.Password!
        );

        if (!success)
            return Conflict("User with the same username or email already exists.");

        return CreatedAtAction(nameof(Login), new { username = registerDto.UserName }, null);
    }
    
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshTokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken()
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _jwtService.RefreshTokenAsync(ipAddress);

        return Ok(result);
    }
    
    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        var result = await _jwtService.ConfirmEmailAsync(userId, token);

        if (!result)
            return BadRequest("Email confirmation failed.");

        return Ok("Email successfully confirmed.");
    }
    
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
    {
        var result = await _jwtService.ForgotPasswordAsync(dto.Email);
        if (!result)
            return NotFound("User not found");
        return Ok("Reset code sent to email.");
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
    {
        var result = await _jwtService.ResetPasswordAsync(dto.Email, dto.Token, dto.NewPassword);
        if (!result)
            return BadRequest("Failed to reset password.");
        return Ok("Password reset successful.");
    }
    
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout()
    {
        await _jwtService.LogoutAsync();
        return Ok(new { message = "Logged out successfully" });
    }
}
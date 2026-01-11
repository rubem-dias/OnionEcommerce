using Microsoft.AspNetCore.Mvc;
using OnionEcommerce.Api.DTOs;
using OnionEcommerce.Application.Services;

namespace OnionEcommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserRegistrationService _registrationService;

    private readonly UserLoginService _loginService;

    public UsersController (
        UserRegistrationService registrationService, 
        UserLoginService loginService
    )
    {
        _registrationService = registrationService;
        _loginService = loginService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var (success, message) = await _registrationService.RegisterUserAsync(
            request.FullName,
            request.Email,
            request.Password);

        if (!success)
            return BadRequest(new { Error = message });

        return Ok(new { Message = message });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var (success, message, token) = await _loginService.LoginAsync(
            request.Email,
            request.Password
        );

        if (!success)
            return BadRequest(new { Error = message });

        return Ok(new { Message = message, Token = token });
    }
}
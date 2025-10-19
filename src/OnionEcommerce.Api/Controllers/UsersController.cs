using Microsoft.AspNetCore.Mvc;
using OnionEcommerce.Api.DTOs;
using OnionEcommerce.Application.Services;

namespace OnionEcommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserRegistrationService _registrationService;

    public UsersController(UserRegistrationService registrationService)
    {
        _registrationService = registrationService;
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
}
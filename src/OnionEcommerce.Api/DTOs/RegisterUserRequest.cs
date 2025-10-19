using System.ComponentModel.DataAnnotations;

namespace OnionEcommerce.Api.DTOs;

public record RegisterUserRequest(
    [Required] string FullName,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password
);
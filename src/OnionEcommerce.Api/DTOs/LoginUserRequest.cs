using System.ComponentModel.DataAnnotations;

namespace OnionEcommerce.Api.DTOs
{
    public record LoginUserRequest(
        [Required][EmailAddress] string Email,
        [Required][MinLength(6)] string Password
    );
}

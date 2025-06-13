using System.ComponentModel.DataAnnotations;

namespace PriceNegotiationAPI.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

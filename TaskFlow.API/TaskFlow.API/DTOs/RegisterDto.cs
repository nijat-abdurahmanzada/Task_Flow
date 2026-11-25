
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8, ErrorMessage = "Password length must be at least 8!")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare(nameof(Password), ErrorMessage ="Passwords do not match!")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

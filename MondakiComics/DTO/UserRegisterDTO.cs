using System.ComponentModel.DataAnnotations;

namespace MondakiComics.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Username { get; set; }

        [Required]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,}$")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel
{
    public class SignUpRequestModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters")]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Mail { get; set; } = string.Empty;
        [Required]
        public DateOnly Birthday { get; set; }
    }
}


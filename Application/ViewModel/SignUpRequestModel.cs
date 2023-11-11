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
        [Required, MinLength(3, ErrorMessage = "Please enter at least 3 characters")]
        public string Password { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Mail { get; set; } = string.Empty;
        [Required]
        public DateOnly Birthday { get; set; }
        [Required]
        public required byte[] HashString { get; set; }
        [Required]
        public required int Code { get; set; }
    }
}


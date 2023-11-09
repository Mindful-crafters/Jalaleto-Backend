using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel
{
    public class EmailRequestModel
    {
        [Required, EmailAddress]
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}

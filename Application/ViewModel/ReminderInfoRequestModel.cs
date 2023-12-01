using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel
{
    public class ReminderInfoRequestModel
    {
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.Reminder
{
    public class ReminderInfoRequestModel
    {
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }

    }
}

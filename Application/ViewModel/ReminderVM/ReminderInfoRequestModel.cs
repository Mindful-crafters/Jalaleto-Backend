using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.ReminderVM
{
    public class ReminderInfoRequestModel
    {
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }

    }
}

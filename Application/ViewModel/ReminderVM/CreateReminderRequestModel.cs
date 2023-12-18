using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.ReminderVM
{
    public class CreateReminderRequestModel
    {
        [Required]
        public required string Title { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int DaysBeforeToRemind { get; set; }

        [Required]
        public bool RemindByEmail { get; set; }

        [Required]
        public PriorityLevel PriorityLevel { get; set; }

        public string? Notes { get; set; }

    }
}

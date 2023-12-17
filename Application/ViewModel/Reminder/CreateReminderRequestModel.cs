using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.Reminder
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

using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Reminder
    {
        [Key]
        public int ReminderId { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public DateTime CreationTimestamp { get; set; }

        public DateTime? LastModificationTimestamp { get; set; }

        public int DaysBeforeToRemind { get; set; }

        public bool RemindByEmail { get; set; }

        public RepeatInterval RepeatInterval { get; set; }

        public PriorityLevel PriorityLevel { get; set; }

        public string? Notes { get; set; }

        public ReminderStatus Status { get; set; }

        // Notification settings (you might need a separate class for this if it's complex)
        // public NotificationSettings NotificationSettings { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

    }
}

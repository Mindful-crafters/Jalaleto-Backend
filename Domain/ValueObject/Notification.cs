using Domain.Entities;
using Domain.Enums;

namespace Domain.ValueObject
{
    public class NotificationModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public NotificationType type { get; set; }
        public Reminder? reminder { get; set; }

        public NotificationModel(int id, string title, string description, NotificationType type, Reminder reminder)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.type = type;
            this.reminder = reminder;
        }
    }
}

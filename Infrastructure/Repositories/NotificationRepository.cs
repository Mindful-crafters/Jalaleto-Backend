using Application.RepositoryInterfaces;
using Application.ViewModel.NotificationVM;
using Domain.Enums;
using Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public NotificationRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<GetNotificationRequestModel> GetNotifications(Guid userId)
        {
            var reminders = await _db.Reminders
                .Where(r => r.UserId == userId
                    && r.DateTime.AddDays(-r.DaysBeforeToRemind) > DateTime.Now
                    && r.Status == ReminderStatus.Active)
                .OrderBy(r => r.DateTime)
                .ToListAsync();

            List<NotificationModel> notifs = new List<NotificationModel>();
            for (int i = 0; i < reminders.Count; i++)
            {
                Domain.Entities.Reminder reminder = reminders[i];
                NotificationModel _notif = new NotificationModel(i, reminder.Title, reminder.Notes, NotificationType.Reminder, reminder);
                notifs.Add(_notif);
            }
            NotificationModel notif = new NotificationModel(10, "تست", "یک متن تستی", NotificationType.Reminder, null);
            notifs.Add(notif);

            return new GetNotificationRequestModel(notifs);
        }
    }
}

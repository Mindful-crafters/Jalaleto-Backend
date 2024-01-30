using Application.RepositoryInterfaces;
using Application.ViewModel.NotificationVM;
using Domain.Entities;
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
            int i = 0;
            List<NotificationModel> notifs = new List<NotificationModel>();

            // Add Reminders
            var reminders = await _db.Reminders
                .Where(r => r.UserId == userId
                    && r.DateTime.AddDays(-r.DaysBeforeToRemind) < DateTime.Now
                    && r.Status == ReminderStatus.Active)
                .OrderBy(r => r.DateTime)
                .ToListAsync();

            for (; i < reminders.Count; i++)
            {
                Domain.Entities.Reminder reminder = reminders[i];
                NotificationModel _notif = new NotificationModel(i, reminder.Title, reminder.Notes, NotificationType.Reminder, reminder);
                notifs.Add(_notif);
            }

            // Add Events
            List<Event> events = new List<Event>();
            var userEvents = await _db.EventsMembers.Where(evm => evm.UserId == userId).ToListAsync();
            foreach (var userEvent in userEvents)
            {
                var _event = await _db.Events
                    .Where(e => e.EventId == userEvent.EventId
                        && DateTime.Now.AddDays(7) < e.When
                        && e.When < DateTime.Now)
                    .FirstOrDefaultAsync();
                if (_event != null)
                {
                    events.Add(_event);
                }
            }

            for (; i < events.Count; i++)
            {
                Event _event = events[i];
                NotificationModel _notif = new NotificationModel(i, _event.Name, _event.Description, NotificationType.Event, _event);
                notifs.Add(_notif);
            }

            return new GetNotificationRequestModel(notifs);
        }
    }
}

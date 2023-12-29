using Application;
using Application.EntityModels;
using Application.RepositoryInterfaces;
using Application.ViewModel.ReminderVM;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public ReminderRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<ApiResponse> CreateReminder(CreateReminderRequestModel request, Guid userId)
        {
            try
            {
                if (request.ReminderId != null)
                {
                    // ReminderId is provided, update the existing reminder
                    var existingReminder = await _db.Reminders.FindAsync(request.ReminderId);

                    if (existingReminder != null)
                    {
                        // Update existing reminder with new values
                        existingReminder.Title = request.Title;
                        existingReminder.DateTime = request.DateTime;
                        existingReminder.DaysBeforeToRemind = request.DaysBeforeToRemind;
                        existingReminder.RemindByEmail = request.RemindByEmail;
                        existingReminder.PriorityLevel = request.PriorityLevel;
                        existingReminder.Notes = request.Notes;
                        existingReminder.Status = ReminderStatus.Active;
                        existingReminder.UserId = userId;

                        // Save changes
                        await _db.SaveChangesAsync();

                        return ApiResponse.Ok();
                    }
                    else
                    {
                        // Handle case where ReminderId is provided but no matching reminder is found
                        return ApiResponse.Error("No existing reminder found with the provided ReminderId.");
                    }
                }
                else
                {
                    // ReminderId is not provided, create a new reminder
                    Reminder reminder = new Reminder(request.Title, request.DateTime, request.DaysBeforeToRemind, request.RemindByEmail,
                        request.PriorityLevel, request.Notes, ReminderStatus.Active, userId);

                    await _db.AddAsync(reminder);
                    await _db.SaveChangesAsync();

                    return ApiResponse.Ok();
                }
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }


        public async Task<ApiResponse> ReminderInfo(ReminderInfoRequestModel request, Guid userId)
        {
            var reminders = await _db.Reminders.Where(r => r.UserId == userId && (r.DateTime >= request.From && r.DateTime <= request.To)).ToListAsync();
            if (reminders == null)
            {
                return ApiResponse.Error("user not found");
            }
            List<ReminderInfo> rem = new List<ReminderInfo>();
            foreach (var item in reminders)
            {
                ReminderInfo info = new ReminderInfo(item.ReminderId, item.Title, item.DateTime,
                    item.PriorityLevel, item.Notes, item.Status);
                rem.Add(info);
            }
            return new ReminderInfoResponseModel(rem);
        }
    }
}

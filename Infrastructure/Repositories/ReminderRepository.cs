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
        public async Task<ApiResponse> CreateReminder(CreateReminderRequestModel request, Guid usreId)
        {
            try
            {
                Reminder reminder = new Reminder(request.Title, request.DateTime, request.DaysBeforeToRemind, request.RemindByEmail,
                request.PriorityLevel, request.Notes, ReminderStatus.Active, usreId);

                await _db.AddAsync(reminder);
                await _db.SaveChangesAsync();

                return ApiResponse.Ok();
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

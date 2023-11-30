using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Domain.Entities;
using Domain.Enums;
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
                request.RepeatInterval, request.PriorityLevel, request.Notes, ReminderStatus.Active, usreId);

                await _db.AddAsync(reminder);
                await _db.SaveChangesAsync();

                return ApiResponse.Ok();
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }


        }
    }
}

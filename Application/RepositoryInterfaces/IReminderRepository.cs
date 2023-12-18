using Application.ViewModel.ReminderVM;

namespace Application.RepositoryInterfaces
{
    public interface IReminderRepository
    {
        public Task<ApiResponse> CreateReminder(CreateReminderRequestModel request, Guid userId);
        public Task<ApiResponse> ReminderInfo(ReminderInfoRequestModel request, Guid userId);
    }
}

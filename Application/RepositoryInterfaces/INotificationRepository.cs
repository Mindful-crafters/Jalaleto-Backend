using Application.ViewModel.NotificationVM;

namespace Application.RepositoryInterfaces
{

    public interface INotificationRepository
    {
        public Task<GetNotificationRequestModel> GetNotifications(Guid userId);

    }

}

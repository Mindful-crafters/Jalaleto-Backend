using Domain.ValueObject;

namespace Application.ViewModel.NotificationVM
{
    public class GetNotificationRequestModel : ApiResponse
    {
        public NotificationModel[] items { get; set; }

        public GetNotificationRequestModel(List<NotificationModel> notifications)
        {
            Success = true;
            Code = 200;
            Message = "Info Returned";
            items = notifications.ToArray();
        }
    }
}

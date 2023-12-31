using Application;
using Application.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Test;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private IHubContext<SignalHub> _hub;
        private readonly INotificationRepository _notificationRepository;
        public NotificationController(INotificationRepository notificationRepository, IHubContext<SignalHub> hub)
        {
            _notificationRepository = notificationRepository;
            _hub = hub;
        }

        [HttpPost("{message}")]
        public void Post(string message)
        {
            _hub.Clients.All.SendAsync("newNotificationRecived", message);
        }

        [HttpPost("{connectionId}/{message}")]
        public void Post(string connectionId, string message)
        {
            _hub.Clients.Client(connectionId).SendAsync("privateMessageMethodName", message);
        }

        [HttpPost]
        [Authorize]
        [Route("Get")]
        public async Task<ApiResponse> GetNotifications()
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _notificationRepository.GetNotifications(Guid.Parse(UserIdString));
        }
    }
}

using Application.RepositoryInterfaces;
using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.ViewModel.ReminderVM;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : Controller
    {
        private readonly IReminderRepository _reminderRepository;
        public ReminderController(IReminderRepository reminderRepository)
        {
            _reminderRepository = reminderRepository;
        }

        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<ApiResponse> CreateReminder(CreateReminderRequestModel request)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if(string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _reminderRepository.CreateReminder(request, Guid.Parse(UserIdString));
        }

        [HttpPost]
        [Authorize]
        [Route("Info")]
        public async Task<ApiResponse> ReminderInfo(ReminderInfoRequestModel request)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _reminderRepository.ReminderInfo(request, Guid.Parse(UserIdString));
        }
    }
}

using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel.EventVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _groupRepository;
        public EventController(IEventRepository eventRepository)
        {
            _groupRepository = eventRepository;
        }
        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<ApiResponse> CreateEvent(CreateEventRequestModel request)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }
            return await _groupRepository.CreateEvent(request, Guid.Parse(UserIdString));
        }
        [HttpPost]
        [Authorize]
        [Route("Events")]
        public async Task<ApiResponse> Events(List<string>? filter)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }
            return await _groupRepository.Events(filter, Guid.Parse(UserIdString));
        }
        [HttpPost]
        [Authorize]
        [Route("Join")]
        public async Task<ApiResponse> JoinEvent(int groupId, Guid eventId)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }
            return await _groupRepository.JoinEvent(groupId, Guid.Parse(UserIdString), eventId);
        }

    }
}

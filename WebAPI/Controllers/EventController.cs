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
        private readonly IEventRepository _eventRepository;
        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
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
            return await _eventRepository.CreateEvent(request, Guid.Parse(UserIdString));
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
            return await _eventRepository.Events(filter, Guid.Parse(UserIdString));
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
            return await _eventRepository.JoinEvent(groupId, Guid.Parse(UserIdString), eventId);
        }

        [HttpPost]
        [Authorize]
        [Route("Info")]
        public async Task<ApiResponse> EventInfo(EventInfoRequestModel request)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }
            return await _eventRepository.EventInfo(request, Guid.Parse(UserIdString));
        }

        [HttpPost]
        [Authorize]
        [Route("AddEventReview")]
        public async Task<ApiResponse> AddEventReview(AddEventReviewRequestModel request)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _eventRepository.AddEventReview(request, Guid.Parse(UserIdString));
        }

        [HttpPost]
        [Authorize]
        [Route("GetEventReviews")]
        public async Task<ApiResponse> GetEventReviews(GetEventReviewsRequestModel request)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _eventRepository.GetEventReviews(request, Guid.Parse(UserIdString));
        }
    }
}

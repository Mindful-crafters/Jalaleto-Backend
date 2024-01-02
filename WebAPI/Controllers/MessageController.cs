using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel.MessageVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpPost]
        [Authorize]
        [Route("SendMessage")]
        public async Task<ApiResponse> SendMessage(SendMessageRequestModel requst)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _messageRepository.SendMessage(requst, Guid.Parse(UserIdString));
        }

        [HttpPost]
        [Authorize]
        [Route("GetMessages")]
        public async Task<ApiResponse> GetMessages(int GroupId)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _messageRepository.GetMessages(GroupId, Guid.Parse(UserIdString));
        }
    }
}

using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;
        public GroupController(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        [HttpPost]
        [Authorize]
        [Route("Create")]
        public async Task<ApiResponse> CreateGroup([FromForm] CreateGroupRequestModel request)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _groupRepository.CreateGroup(request, Guid.Parse(UserIdString));
        }
        
    }
}

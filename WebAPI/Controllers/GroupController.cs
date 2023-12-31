using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel.GroupVM;
using Microsoft.AspNetCore.Authorization;
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
        [HttpPost]
        [Authorize]
        [Route("GpInfo")]
        public async Task<ApiResponse> GetSingleGroupInfo(int GroupId)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _groupRepository.GetSingleGroupInfo(GroupId, Guid.Parse(UserIdString));
        }
        [HttpPost]
        [Authorize]
        [Route("Search")]
        public async Task<ApiResponse> SearchGroups(string GroupName)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _groupRepository.SearchGroups(GroupName, Guid.Parse(UserIdString));
        }
        [HttpPost]
        [Authorize]
        [Route("MyGroups")]
        public async Task<ApiResponse> GroupInfo()
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _groupRepository.GroupInfo(Guid.Parse(UserIdString));
        }
        [HttpPost]
        [Authorize]
        [Route("UploadImage")]
        public async Task<ApiResponse> UploadImage(IFormFile image,int groupId)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }

            return await _groupRepository.UploadImage(image, Guid.Parse(UserIdString), groupId);    
        }
        [HttpPost]
        [Route("PopularGroups")]
        public async Task<ApiResponse> PopularGroups(int cnt)
        {
            return await _groupRepository.PopularGroups(cnt);
        }

    }
}

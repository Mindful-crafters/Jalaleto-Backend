using Application.ViewModel.GroupVM;
using Microsoft.AspNetCore.Http;

namespace Application.RepositoryInterfaces
{

    public interface IGroupRepository
    {
        public Task<ApiResponse> CreateGroup(CreateGroupRequestModel request, Guid userId);
        public Task<ApiResponse> GroupsInfo(Guid userId, bool filterMyGroups);
        public Task<ApiResponse> UploadImage(IFormFile Image, Guid userId, int groupId);
        public Task<ApiResponse> PopularGroups(int cnt);
        public Task<ApiResponse> GetSingleGroupInfo(int GroupId, Guid userId);
        public Task<ApiResponse> SearchGroups(string pattern, Guid userId);
        public Task<ApiResponse> JoinGroup(int GroupId, Guid userId);
    }

}

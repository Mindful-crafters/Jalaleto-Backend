using Application.ViewModel.GroupVM;

namespace Application.RepositoryInterfaces
{

    public interface IGroupRepository
    {
        public Task<ApiResponse> CreateGroup(CreateGroupRequestModel request, Guid userId);
        public Task<ApiResponse> GroupInfo(Guid userId);
    }

}

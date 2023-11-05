using Application.ViewModel;
using Domain.Entities;

namespace Application.RepositoryInterfaces
{
    public interface IUserRepository
    {
        public Task<ApiResponse> Login(string username, string password);
        public Task<ApiResponse> SignUp(SignUpRequestModel request);
    }
}

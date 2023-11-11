using Application.ViewModel;
using Domain.Entities;

namespace Application.RepositoryInterfaces
{
    public interface IUserRepository
    {
        public Task<ApiResponse> Login(LoginRequestModel request);
        public Task<ApiResponse> SignUp(SignUpRequestModel request);
    }
}

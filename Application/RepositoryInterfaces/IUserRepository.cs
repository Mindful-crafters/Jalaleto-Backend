using Application.ViewModel;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.RepositoryInterfaces
{
    public interface IUserRepository
    {
        public Task<ApiResponse> Login(LoginRequestModel request);
        public Task<ApiResponse> SignUp(SignUpRequestModel request);
        public Task<ApiResponse> SendVerifyEmail(SendVerifyEmailRequestModel request);
        public Task<ApiResponse> SendRestPasswordEmail(SendVerifyEmailRequestModel request);
        public Task<ApiResponse> ResetPassword(ResetPasswordRequestModel request);
        public Task<ApiResponse> CheckEmail(SendVerifyEmailRequestModel request);
        public Task<ApiResponse> ProfileInfo(string JwtToken);
        public Task<ApiResponse> EditProfileInfo(EditProfileInfoRequestModel request);
        public Task<ApiResponse> UploadImage(IFormFile request,string JwtToken);
        
    }
}
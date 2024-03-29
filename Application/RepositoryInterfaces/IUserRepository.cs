﻿using Application.ViewModel.UserVM;

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
        public Task<ApiResponse> ProfileInfo(Guid UserId);
        public Task<ApiResponse> EditProfileInfo(EditProfileInfoRequestModel request, Guid UserId);
        public Task<ApiResponse> LandingInfo();
        //  public Task<ApiResponse> UploadImage(IFormFile request , Guid UserId);

    }
}
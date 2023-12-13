using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<ApiResponse> Singup(SignUpRequestModel request)
        {
            return await _userRepository.SignUp(request);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ApiResponse> Login(LoginRequestModel request)
        {
            return await _userRepository.Login(request);
        }

        [HttpPost]
        [Route("SendVerifyEmail")]
        public async Task<ApiResponse> SendVerifyEmail(SendVerifyEmailRequestModel request)
        {
            return await _userRepository.SendVerifyEmail(request);
        }

        [HttpPost]
        [Route("SendRestPasswordEmail")]
        public async Task<ApiResponse> SendRestPasswordEmail(SendVerifyEmailRequestModel request)
        {
            return await _userRepository.SendRestPasswordEmail(request);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<ApiResponse> ResetPassword(ResetPasswordRequestModel request)
        {
            return await _userRepository.ResetPassword(request);

        }
        [HttpPost]
        [Route("CheckEmail")]
        public async Task<ApiResponse> CheckEmail(SendVerifyEmailRequestModel request)
        {
            return await _userRepository.CheckEmail(request);

        }
        [HttpPost]
        [Authorize]
        [Route("ProfileInfo")]
        public async Task<ApiResponse> ProfileInfo()
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }
            return await _userRepository.ProfileInfo(Guid.Parse(UserIdString));
        }
        [HttpPost]
        [Authorize]
        [Route("EditProfile")]
        public async Task<ApiResponse> EditProfileInfo(EditProfileInfoRequestModel request)
        {
            string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
            if (string.IsNullOrWhiteSpace(UserIdString))
            {
                return ApiResponse.Unauthorized();
            }
            return await _userRepository.EditProfileInfo(request, Guid.Parse(UserIdString));
        }
       
        //[HttpPost]
        //[Authorize]
        //[Route("UploadImage")]
        //public async Task<ApiResponse> UploadImage(IFormFile file)
        //{
        //    string UserIdString = User.Claims.First(x => x.Type == "UserId").Value;
        //    if (string.IsNullOrWhiteSpace(UserIdString))
        //    {
        //        return ApiResponse.Unauthorized();
        //    }
        //    return await _userRepository.UploadImage(file, Guid.Parse(UserIdString));
        //}


    }
}

using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        public UserController(IUserRepository _userRepository)
        {
            userRepository = _userRepository;
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<ApiResponse> Singup(SignUpRequestModel request)
        {
            return await userRepository.SignUp(request);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ApiResponse> Login()
        {
            return ApiResponse.Ok();
        }

    }
}

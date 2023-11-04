using Application;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        [HttpPost]
        [Route("SignUp")]
        public ApiResponse Singup()
        {
            return ApiResponse.Ok();
        }

    }
}

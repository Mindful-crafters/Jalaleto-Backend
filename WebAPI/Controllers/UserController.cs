using Application;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db) 
        {
            _db = db;
        }

        [HttpPost]
        public ApiResponse Create(User u)
        {
            try
            {
                _db.Add(u);
                _db.SaveChanges();
                return ApiResponse.Ok();
            }
            catch (Exception)
            {

                return ApiResponse.Error();
            }

            
        }




        //dummy 
        [HttpPost]
        [Route("SignUp")]
        public ApiResponse Singup()
        {
            return ApiResponse.Ok();
        }

    }
}

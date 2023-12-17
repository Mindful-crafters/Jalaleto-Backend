using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.User
{
    public class LoginResponseModel : ApiResponse
    {
        public LoginResponseModel(string token)
        {
            Success = true;
            Code = 200;
            Message = "Logged in";
            Token = token;
        }
        public string Token { get; set; }
    }
}

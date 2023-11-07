using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class LoginResponseModel: ApiResponse
    {
        public LoginResponseModel(string token)
        {
            this.Success = true;
            this.Code = 200;
            this.Message = "Logged in";
            this.Token = token;
        }
        public string Token { get; set; }
    }
}

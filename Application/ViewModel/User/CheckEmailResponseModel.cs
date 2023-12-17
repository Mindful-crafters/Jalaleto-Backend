using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.User
{
    public class CheckEmailResponseModel : ApiResponse
    {
        public bool Exists { get; set; }
        public CheckEmailResponseModel(bool exists)
        {
            Success = true;
            Code = 200;
            Message = "success";
            Exists = exists;

        }
    }
}


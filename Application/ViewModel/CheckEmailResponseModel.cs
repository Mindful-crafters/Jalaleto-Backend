using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class CheckEmailResponseModel : ApiResponse
    {
        public bool Exists { get; set; }
        public CheckEmailResponseModel(bool exists)
        {
            this.Success = true;
            this.Code = 200;
            this.Message = "success";
            this.Exists = exists;

        }
    }
}
        

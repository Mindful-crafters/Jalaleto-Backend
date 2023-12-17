using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.User
{
    public class SendVerifyEmailRequestModel
    {
        public required string email { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.User
{
    public class ResetPasswordRequestModel
    {
        [EmailAddress]
        public string Mail { get; set; } = string.Empty;
        public int Code { get; set; }
        public string NewPassword { get; set; } = string.Empty;
        public required byte[] HashString { get; set; }
    }
}

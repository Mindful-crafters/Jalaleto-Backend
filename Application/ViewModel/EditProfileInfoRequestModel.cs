using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class EditProfileInfoRequestModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateOnly Birthday { get; set; }
        public string JwtToken { get; set; } = string.Empty; // token that was generated when user logged in
    }
}

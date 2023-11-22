using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class ProfileInfoResponseModel : ApiResponse
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Birthday { get; set; }

        public ProfileInfoResponseModel(string FirstName, string LastName, string Username, string birthday)
        {
            this.Success = true;
            this.Code = 200;
            this.Message = "Info Returned";
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.UserName = Username;
            this.Birthday = birthday;
        }
    }
}

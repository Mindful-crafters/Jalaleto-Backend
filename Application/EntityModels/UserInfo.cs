using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EntityModels
{
    public class UserInfo
    {
        public string Mail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime Birthday { get; set; }
        public string Image { get; set; }
        public UserInfo(string mail, string firstname, string lastname, string username, DateTime birthday)
        {
            Mail =  mail;
            FirstName = firstname;
            LastName = lastname;
            UserName = username;
            Birthday = birthday;

        }
    }
}

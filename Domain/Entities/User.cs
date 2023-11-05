using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public User()
        {
            
        }
        public User(string firstname,string lastname,string username, string password,string mail,DateTime birthday)
        {
            FirstName = firstname;
            LastName = lastname;
            UserName = username;
            Password = password;
            Mail = mail;
            Brithday = birthday;
        }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime Brithday { get; set; }
    }
}

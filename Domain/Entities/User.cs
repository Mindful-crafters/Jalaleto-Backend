using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public User(string firstname,string lastname,string username, string password,string mail,DateOnly birthday)
        {
            FirstName = firstname;
            LastName = lastname;
            UserName = username;
            Password = password;
            Mail = mail;
            Birthday = birthday.ToDateTime(TimeOnly.Parse("10:00 PM"));
        }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        [Column(TypeName = "Date")]
        public DateTime Birthday { get; set; }
    }
}

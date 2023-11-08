using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User
    {
        public User()
        {

        }
        public User(string firstname, string lastname, string username, byte[] passwordHash, byte[] passwordSalt, string mail, DateOnly birthday)
        {
            FirstName = firstname;
            LastName = lastname;
            UserName = username;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            Mail = mail;
            Birthday = birthday.ToDateTime(TimeOnly.Parse("10:00 PM"));
        }
        public Guid Id { get; set; }
        public string Mail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        [Column(TypeName = "Date")]
        public DateTime Birthday { get; set; }
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        
    }
}

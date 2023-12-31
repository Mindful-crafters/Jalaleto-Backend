using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Event
    {
        [Key]
        public Guid EventId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public DateTime When { get; set; }
        [Required]
        public Guid Owner { get; set; }
        public string? Location { get; set; }
        [Required]
        public int MemberLimit { get; set; }
        public string Tag { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public Event()
        {
            
        }
        public Event(string name, string desc, DateTime when, string location, int memlimit, string tag, Guid owner)
        {
            Name = name;
            Description = desc;
            When = when;
            Location = location;
            MemberLimit = memlimit;
            Tag = tag;
            Owner = owner;
           
        }

    }
}

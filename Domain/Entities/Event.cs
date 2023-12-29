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
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
        public string? Location { get; set; }
        [Required]
        public int MemberLimit { get; set; }

    }
}

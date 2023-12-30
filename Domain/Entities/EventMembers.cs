using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EventMembers
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int GroupId { get; set; }
        public Guid EventId { get; set; }
        public EventMembers(Guid userId, Guid eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
    }
}

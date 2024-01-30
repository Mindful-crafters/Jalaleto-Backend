using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class EventReview
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public string? Text { get; set; }
        

        [Required]
        public Guid EventId { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }


        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public EventReview(int score, string? text, Guid eventId, Guid userId)
        {
            Score = score;
            Text = text;
            EventId = eventId;
            UserId = userId;
        }
    }
}

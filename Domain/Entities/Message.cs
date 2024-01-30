using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Message
    {
        [Required]
        [Key]
        public int MessageId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public Guid SenderUserId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime SentTime { get; set; } = DateTime.Now;

        public Message()
        {
        }

        public Message(int groupId, Guid senderUserId, string content)
        {
            GroupId = groupId;
            SenderUserId = senderUserId;
            Content = content;
        }
    }
}

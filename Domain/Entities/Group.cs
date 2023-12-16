using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{

    public class Group
    {
        [Required]
        [Key]
        public int GroupId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid Owner { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public Group()
        {

        }
        public Group(string name, Guid owner)
        {
            Name = name;
            Owner = owner;
        }

    }

}

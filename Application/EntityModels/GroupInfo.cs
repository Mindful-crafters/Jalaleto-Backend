using Domain.Entities;

namespace Application.EntityModels
{
    public class GroupInfo
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<UserInfo> Members { get; set; }
        public List<EventInfo> Events { get; set; }

        public GroupInfo(int gid, string name, string desc, string imageurl, List<UserInfo> members, List<EventInfo> events)
        {
            GroupId = gid;
            Name = name;
            Description = desc;
            ImageUrl = imageurl;
            Members = members;
            Events = events;
        }
    }
}

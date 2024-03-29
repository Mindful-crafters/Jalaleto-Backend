﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EntityModels
{
    public class EventInfo
    {
        public int GroupId { get; set; }
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime When { get; set; } 
        public List<UserInfo> Members { get; set; }
        public int MemberLimit { get; set; }
        public List<string> Tag { get; set; } 
        public bool Maked { get; set; }

        public EventInfo(Guid eid, string name, string desc, DateTime When, List<UserInfo> members, 
            int memberlim , List<string>  tags, int groupId, bool maked)
        {
            EventId = eid;
            Name = name;
            Description = desc;
            
            Members = members;
            this.When = When;
            MemberLimit = memberlim;
            Tag = tags;
            GroupId = groupId;
            Maked = maked;

        }
    }
}

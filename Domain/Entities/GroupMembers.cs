﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class GroupMembers
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Guid UserId { get; set; }
        public GroupMembers()
        {

        }
        public GroupMembers(int groupId, Guid userId)
        {
            this.GroupId = groupId;
            this.UserId = userId;
        }
    }
}

﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EntityModels
{
    public class GroupInfo
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Members { get; set; }

        public GroupInfo(int gid, string name, string desc, string imageurl, List<string> members)
        {
            GroupId = gid;
            Name = name;
            Description = desc;
            ImageUrl = imageurl;
            Members = members;
        }
    }
}

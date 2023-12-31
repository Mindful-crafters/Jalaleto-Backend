﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.EventVM
{
    public class CreateEventRequestModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime When { get; set; }
        public string? Location { get; set; }
        public int MemberLimit { get; set; }
        public string Tag { get; set; } = string.Empty;

    }
}
﻿using Microsoft.AspNetCore.Http;

namespace Application.ViewModel.UserVM
{
    public class EditProfileInfoRequestModel
    {
        //public string ImagePath { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateOnly Birthday { get; set; }
        public IFormFile? image { get; set; }
        public List<string>? Interests { get; set; }
    }
}

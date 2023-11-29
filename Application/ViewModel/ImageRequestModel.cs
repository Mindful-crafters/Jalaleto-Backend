using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Application.ViewModel
{
    public class ImageRequestModel
    {
        public string Name { get; set; }
        public IFormFile image { get; set; }
    }
}

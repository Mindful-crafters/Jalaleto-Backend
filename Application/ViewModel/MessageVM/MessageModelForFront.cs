using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.MessageVM
{
    public class MessageModelForFront : Message
    {
        public string SenderName { get; set; }
        public string SenderImageUrl { get; set; }
        public bool AreYouSender { get; set; }
    }
}

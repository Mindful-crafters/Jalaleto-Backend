using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.MessageVM
{
    public class SendMessageRequestModel
    {
        public string Message { get; set; }
        public int GroupId { get; set; }
    }
}

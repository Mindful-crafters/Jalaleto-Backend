using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.EventVM
{
    public class AddEventReviewRequestModel
    {
        public int Score { get; set; }
        public string? Text { get; set; }
        public Guid EventId { get; set; }
    }
}

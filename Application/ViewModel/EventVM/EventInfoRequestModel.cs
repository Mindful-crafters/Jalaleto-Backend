using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.EventVM
{
    public class EventInfoRequestModel
    {
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
    }
}

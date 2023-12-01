using Application.EntityModels;
using Domain.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class ReminderInfoResponseModel : ApiResponse
    {
        [JsonProperty("Data")]
        public List<ReminderInfo> Data { get; set; }
        
        public ReminderInfoResponseModel(List<ReminderInfo> reminders)
        {
            this.Success = true;
            this.Code = 200;
            this.Message = "Info Returned";
            this.Data = reminders;
            
        }
       
    }
}

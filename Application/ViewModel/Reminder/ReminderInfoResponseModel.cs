using Application.EntityModels;
using Domain.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.Reminder
{
    public class ReminderInfoResponseModel : ApiResponse
    {
        [JsonProperty("Data")]
        public List<ReminderInfo> Data { get; set; }

        public ReminderInfoResponseModel(List<ReminderInfo> reminders)
        {
            Success = true;
            Code = 200;
            Message = "Info Returned";
            Data = reminders;

        }

    }
}

using Application.EntityModels;
using Newtonsoft.Json;

namespace Application.ViewModel.ReminderVM
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

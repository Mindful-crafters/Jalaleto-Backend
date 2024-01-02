using Application.EntityModels;
using Newtonsoft.Json;

namespace Application.ViewModel.GroupVM
{
    public class GroupInfoResponseModel : ApiResponse
    {
        [JsonProperty("Data")]
        public List<GroupInfo> Data { get; set; } = new List<GroupInfo>();

        public GroupInfoResponseModel(List<GroupInfo> groups)
        {
            Success = true;
            Code = 200;
            Message = "Info Returned";
            Data = groups;
        }

        public GroupInfoResponseModel(GroupInfo groups)
        {
            Success = true;
            Code = 200;
            Message = "Info Returned";
            Data.Add(groups);

        }
    }
}

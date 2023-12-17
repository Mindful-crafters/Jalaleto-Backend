using Application.EntityModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.Group
{
    public class GroupInfoResponseModel : ApiResponse
    {
        [JsonProperty("Data")]
        public List<GroupInfo> Data { get; set; }

        public GroupInfoResponseModel(List<GroupInfo> groups)
        {
            Success = true;
            Code = 200;
            Message = "Info Returned";
            Data = groups;

        }
    }
}

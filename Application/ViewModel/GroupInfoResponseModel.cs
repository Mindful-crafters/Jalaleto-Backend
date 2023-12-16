using Application.EntityModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class GroupInfoResponseModel : ApiResponse
    {
        [JsonProperty("Data")]
        public List<GroupInfo> Data { get; set; }

        public GroupInfoResponseModel(List<GroupInfo> groups)
        {
            this.Success = true;
            this.Code = 200;
            this.Message = "Info Returned";
            this.Data = groups;

        }
    }
}

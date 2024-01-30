using Application.EntityModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Application.ViewModel.EventVM
{
    public class EventInfoResponseModel : ApiResponse
    {
        public List<EventInfo> Data { get; set; } = new List<EventInfo>();

        public EventInfoResponseModel(List<EventInfo> events)
        {
            Success = true;
            Code = 200;
            Message = "Info Returned";
            Data = events;
        }
    }
}

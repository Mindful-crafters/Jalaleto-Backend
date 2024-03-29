﻿using Application.EntityModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Application.ViewModel.MessageVM
{
    public class GetMessageResponseModel: ApiResponse
    {
        public List<MessageModelForFront> Data { get; set; } = new List<MessageModelForFront>();

        public GetMessageResponseModel(List<MessageModelForFront> messages)
        {
            Success = true;
            Code = 200;
            Message = "Info Returned";
            Data = messages;
        }
    }
}

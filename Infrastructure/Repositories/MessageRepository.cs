using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel.MessageVM;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MessageRepository: IMessageRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public MessageRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<ApiResponse> SendMessage(SendMessageRequestModel request, Guid userId)
        {
            try
            {
                var group = await _db.Groups
                    .Where((group) => group.GroupId == request.GroupId)
                    .ToListAsync();

                if (group.Count == 0)
                {
                    throw new Exception("Group id is not valid!");
                }

                var membership = await _db.GroupMembers
                    .Where((member) => member.UserId == userId && member.GroupId == request.GroupId)
                    .ToListAsync();

                if (membership.Count == 0)
                {
                    throw new Exception("User is not member of group!");
                }

                Message message = new Message(request.GroupId, userId, request.Message);
                await _db.Messages.AddAsync(message);
                await _db.SaveChangesAsync();

                return ApiResponse.Ok("Message sent!");

            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> GetMessages(int groupId, Guid userId)
        {
            try
            {
                var group = await _db.Groups
                    .Where((group) => group.GroupId == groupId)
                    .ToListAsync();

                if (group.Count == 0)
                {
                    throw new Exception("Group id is not valid!");
                }

                var membership = await _db.GroupMembers
                    .Where((member) => member.UserId == userId && member.GroupId == groupId)
                    .ToListAsync();

                if (membership.Count == 0)
                {
                    throw new Exception("User is not member of group!");
                }

                List<Message> messages = 
                    await _db.Messages
                        .Where((message) => message.GroupId == groupId)
                        .OrderBy((message) => message.SentTime)
                        .ToListAsync();

                return new GetMessageResponseModel(messages);

            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }
    }
}

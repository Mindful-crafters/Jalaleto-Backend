using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel.MessageVM;
using Domain.Entities;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _db;
        private IHubContext<MessageHub> _messageHub;

        public MessageRepository(ApplicationDbContext db, IConfiguration configuration, IHubContext<MessageHub> messageHub)
        {
            _db = db;
            _messageHub = messageHub;
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

                Message messageForDataBase = new Message(request.GroupId, userId, request.Message);
                await _db.Messages.AddAsync(messageForDataBase);
                await _db.SaveChangesAsync();

                MessageModelForFront messageForFront = new MessageModelForFront
                {
                    MessageId = messageForDataBase.MessageId,
                    GroupId = messageForDataBase.GroupId,
                    SenderUserId = messageForDataBase.SenderUserId,
                    Content = messageForDataBase.Content,
                    SentTime = messageForDataBase.SentTime,
                    SenderName = _db.Users.Where(u => u.Id == messageForDataBase.SenderUserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault()!,
                    SenderImageUrl = _db.Users.Where(u => u.Id == messageForDataBase.SenderUserId).Select(u => u.ImagePath).FirstOrDefault()!,
                    AreYouSender = (messageForDataBase.SenderUserId == userId), // Replace with the actual user ID
                };
                await _messageHub.Clients.Groups(request.GroupId.ToString()).SendAsync("NewMessage", messageForFront);
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

                List<MessageModelForFront> messages = await _db.Messages
                    .Where(message => message.GroupId == groupId)
                    .OrderBy(message => message.SentTime)
                    .Select(message => new MessageModelForFront
                    {
                        MessageId = message.MessageId,
                        GroupId = message.GroupId,
                        SenderUserId = message.SenderUserId,
                        Content = message.Content,
                        SentTime = message.SentTime,
                        SenderName = _db.Users.Where(u => u.Id == message.SenderUserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault()!,
                        SenderImageUrl = _db.Users.Where(u => u.Id == message.SenderUserId).Select(u => u.ImagePath).FirstOrDefault()!,
                        AreYouSender = (message.SenderUserId == userId), // Replace with the actual user ID
                    })
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

using Application.ViewModel.MessageVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RepositoryInterfaces
{
    public interface IMessageRepository
    {
        public Task<ApiResponse> SendMessage(SendMessageRequestModel request, Guid userId);
        public Task<ApiResponse> GetMessages(int GroupId, Guid userId);
    }
}

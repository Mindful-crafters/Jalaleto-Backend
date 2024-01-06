using Application.ViewModel.EventVM;
using Application.ViewModel.GroupVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RepositoryInterfaces
{
    public interface IEventRepository
    {
        public Task<ApiResponse> CreateEvent(CreateEventRequestModel request, Guid userId);
        public Task<ApiResponse> Events(List<string> filter, Guid userId);

    }
}

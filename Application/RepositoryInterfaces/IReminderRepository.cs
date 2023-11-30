using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RepositoryInterfaces
{
    public interface IReminderRepository
    {
        public Task<ApiResponse> CreateReminder(CreateReminderRequestModel request, Guid userId);
    }
}

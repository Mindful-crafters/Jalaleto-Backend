using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RepositoryInterfaces
{
    
    public interface IGroupRepository
    {
        public Task<ApiResponse> CreateGroup(CreateGroupRequestModel request, Guid userId);
      //  public Task<ApiResponse> GroupInfo(Guid userId);
    }
    
}

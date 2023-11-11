using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RepositoryInterfaces
{
    public interface IEmailRepository
    {
        public Task<ApiResponse> SendMail(EmailRequestModel request);
    }
}

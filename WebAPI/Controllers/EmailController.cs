using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;

        public EmailController(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }
        [HttpPost]
        [Route("SendMail")]
        public async Task<ApiResponse> SendMail(EmailRequestModel request)
        {
            return await _emailRepository.SendMail(request);
        }
    }
}

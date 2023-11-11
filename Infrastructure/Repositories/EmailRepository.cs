using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _config;

        public EmailRepository(IConfiguration config)
        {
            _config = config;
        }
        public async Task<ApiResponse> SendMail(EmailRequestModel request)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

                using var smtp = new SmtpClient();
                smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                smtp.Send(email);
                smtp.Disconnect(true);
                return ApiResponse.Ok();
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }
    }
}

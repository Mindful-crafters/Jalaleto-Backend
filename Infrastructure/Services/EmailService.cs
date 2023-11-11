using Application.ViewModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Services
{
    public static class EmailService
    {
        static IConfiguration _config = ConfigurationHelper.config;
        public static void SendMail(string email, string subject, string body)
        {
            var emailModel = new MimeMessage();
            emailModel.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            emailModel.To.Add(MailboxAddress.Parse(email));
            emailModel.Subject = subject;
            emailModel.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Send(emailModel);
            smtp.Disconnect(true);
        }
    }
}

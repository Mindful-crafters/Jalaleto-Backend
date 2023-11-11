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
        public async static Task SendMail(string email, string subject, string body)
        {
            var emailModel = new MimeMessage();
            emailModel.From.Add(MailboxAddress.Parse(_config.GetSection("AppSettings.EmailUsername").Value));
            emailModel.To.Add(MailboxAddress.Parse(email));
            emailModel.Subject = subject;
            emailModel.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config.GetSection("AppSettings.EmailHost").Value, 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config.GetSection("AppSettings.EmailUsername").Value, _config.GetSection("AppSettings.EmailPassword").Value);
            await smtp.SendAsync(emailModel);
            await smtp.DisconnectAsync(true);
        }
    }
}

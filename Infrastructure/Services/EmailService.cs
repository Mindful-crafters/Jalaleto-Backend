using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Services
{
    public static class EmailService
    {
        public async static Task SendMail(IConfiguration _config, string email, string subject, string body)
        {
            var emailModel = new MimeMessage();

            emailModel.From.Add(MailboxAddress.Parse(_config.GetSection("AppSettings:EmailUsername").Value));
            emailModel.To.Add(MailboxAddress.Parse(email));
            emailModel.Subject = subject;
            emailModel.Body = new TextPart(TextFormat.Html) { Text = body };

            using SmtpClient client = new(new ProtocolLogger("smtp.log"));
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(_config.GetSection("AppSettings:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config.GetSection("AppSettings:EmailUsername").Value, _config.GetSection("AppSettings:EmailPassword").Value);
            await client.SendAsync(emailModel);
            await client.DisconnectAsync(true);

        }
    }
}

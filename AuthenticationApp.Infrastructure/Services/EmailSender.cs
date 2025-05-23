using AuthenticationApp.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace AuthenticationApp.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var from = _config["Email:From"];
            var smtp = _config["Email:SmtpServer"];
            var port = int.Parse(_config["Email:Port"]!);
            var user = _config["Email:Username"];
            var pass = _config["Email:Password"];

            var message = new MailMessage(from, to, subject, body) { IsBodyHtml = true };

            using var client = new SmtpClient(smtp, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}

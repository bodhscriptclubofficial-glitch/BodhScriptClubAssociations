using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BodhScriptClubOfficialAPI.Repositories
{
    public class EmailServicecs
    {
        private readonly string _fromEmail;
        private readonly string _appPassword;

        public EmailServicecs(IConfiguration configuration)
        {
            _fromEmail = configuration["GMAIL_EMAIL"]
                         ?? throw new InvalidOperationException("FromEmail not set in configuration");
            _appPassword = configuration["GMAIL_APP_PASSWORD"]
                           ?? throw new InvalidOperationException("AppPassword not set in configuration");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, "Bodh Script Club"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(_fromEmail, _appPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }

}
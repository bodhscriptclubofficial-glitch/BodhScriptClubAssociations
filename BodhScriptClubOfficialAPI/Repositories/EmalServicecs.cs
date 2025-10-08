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

        public EmailServicecs()
        {
            _fromEmail = Environment.GetEnvironmentVariable("GMAIL_EMAIL")
                         ?? "bodhscriptclubofficial@gmail.com";
            _appPassword = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD")
                           ?? throw new InvalidOperationException("GMAIL_APP_PASSWORD not set");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required", nameof(toEmail));

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

            try
            {
                await client.SendMailAsync(message);
                Console.WriteLine($"✅ Email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending email: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
            }
        }
    }

}
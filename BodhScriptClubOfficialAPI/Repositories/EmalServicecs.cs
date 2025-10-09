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
        private readonly string _smtpLogin;
        private readonly string _smtpPassword;

        public EmailServicecs()
        {
            _fromEmail = Environment.GetEnvironmentVariable("BREVO_EMAIL")
                ?? throw new InvalidOperationException("BREVO_EMAIL not set in Render environment");

            _smtpLogin = Environment.GetEnvironmentVariable("BREVO_SMTP_LOGIN")
                ?? throw new InvalidOperationException("BREVO_SMTP_LOGIN not set in Render environment");

            _smtpPassword = Environment.GetEnvironmentVariable("BREVO_SMTP_PASSWORD")
                ?? throw new InvalidOperationException("BREVO_SMTP_PASSWORD not set in Render environment");
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

            using var client = new SmtpClient("smtp-relay.brevo.com", 587)
            {
                Credentials = new NetworkCredential(_smtpLogin, _smtpPassword),
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
            }
        }
    }

}
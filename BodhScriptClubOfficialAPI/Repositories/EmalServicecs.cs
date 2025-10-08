using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BodhScriptClubOfficialAPI.Repositories
{
    public class EmailServicecs
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required", nameof(toEmail));
            var fromEmail = Environment.GetEnvironmentVariable("BREVO_SMTP_LOGIN");
            var smtpKey = Environment.GetEnvironmentVariable("BREVO_SMTP_KEY");

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, "Bodh Script Club"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            using var client = new SmtpClient("smtp-relay.brevo.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, smtpKey),
                EnableSsl = true
            };



            await client.SendMailAsync(message);
        }
    }
}

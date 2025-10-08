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

            // Get environment variables
            var fromEmail = Environment.GetEnvironmentVariable("BREVO_SMTP_LOGIN");
            var smtpKey = Environment.GetEnvironmentVariable("BREVO_SMTP_KEY");

            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new InvalidOperationException("Environment variable 'BREVO_SMTP_LOGIN' is not set.");
            if (string.IsNullOrWhiteSpace(smtpKey))
                throw new InvalidOperationException("Environment variable 'BREVO_SMTP_KEY' is not set.");

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
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            try
            {
                Console.WriteLine($"Sending email from '{fromEmail}' to '{toEmail}'...");
                await client.SendMailAsync(message);
                Console.WriteLine("✅ Email sent successfully!");
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"❌ SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ General Error: {ex.Message}");
                throw;
            }
        }
    }
}

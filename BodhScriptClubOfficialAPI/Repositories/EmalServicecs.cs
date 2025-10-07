using System.Net;
using System.Net.Mail;

namespace BodhScriptClubOfficialAPI.Repositories
{
    public class EmailServicecs
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromEmail = "bodhscriptclubofficial@gmail.com"; // your verified Brevo email
            var brevoApiKey = Environment.GetEnvironmentVariable("SENDINBLUE_API_KEY"); // must be set in Render

            if (string.IsNullOrEmpty(brevoApiKey))
                throw new Exception("SENDINBLUE_API_KEY environment variable is not set.");

            using var message = new MailMessage();
            message.From = new MailAddress(fromEmail, "Bodh Script Club");
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var client = new SmtpClient("smtp-relay.brevo.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, brevoApiKey),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}

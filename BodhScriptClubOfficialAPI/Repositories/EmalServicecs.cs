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
        private readonly string _apiKey;
        private readonly string _fromEmail;

        public EmailServicecs()
        {
            _apiKey = Environment.GetEnvironmentVariable("SENDINBLUE_API_KEY")
                      ?? throw new InvalidOperationException("SENDINBLUE_API_KEY environment variable not set.");
            _fromEmail = Environment.GetEnvironmentVariable("BREVO_SMTP_LOGIN")
                         ?? "98bbef002@smtp-brevo.com"; // fallback if env not set
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required", nameof(toEmail));

            var payload = new
            {
                sender = new { name = "Bodh Script Club", email = _fromEmail },
                to = new[] { new { email = toEmail } },
                subject = subject,
                htmlContent = htmlBody
            };

            var json = JsonConvert.SerializeObject(payload);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("api-key", _apiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://api.brevo.com/v3/smtp/email", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Email sent to {toEmail}");
                }
                else
                {
                    var respText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Email failed: {response.StatusCode} - {respText}");
                }
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
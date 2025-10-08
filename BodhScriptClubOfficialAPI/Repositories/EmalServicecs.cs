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

        public EmailServicecs()
        {
            _apiKey = Environment.GetEnvironmentVariable("BREVO_SMTP_KEY"); // same key works for API
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("BREVO_SMTP_KEY environment variable not set.");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var fromEmail = Environment.GetEnvironmentVariable("BREVO_SMTP_LOGIN") ?? "98bbef002@smtp-brevo.com";

            var payload = new
            {
                sender = new { name = "Bodh Script Club", email = fromEmail },
                to = new[] { new { email = toEmail } },
                subject = subject,
                htmlContent = htmlBody
            };

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("api-key", _apiKey);
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await http.PostAsync("https://api.brevo.com/v3/smtp/email", content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("✅ Email sent successfully!");
            else
                Console.WriteLine($"❌ Email send failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
    }

}
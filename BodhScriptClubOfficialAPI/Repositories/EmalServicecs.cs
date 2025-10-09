using brevo_csharp.Api;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace BodhScriptClubOfficialAPI.Repositories
{
    public class EmailServicecs
    {
        private readonly string _fromEmail;
        private readonly string _apiKey;
        private readonly TransactionalEmailsApi _emailApi;

        public EmailServicecs(IConfiguration configuration)
        {
            _fromEmail = Environment.GetEnvironmentVariable("BREVO_FROM_EMAIL")
              ?? "bodhscriptclubofficial@gmail.com";
            _apiKey = Environment.GetEnvironmentVariable("BREVO_SMTP_KEY")
                      ?? configuration["Brevo:ApiKey"]
                      ?? throw new InvalidOperationException("BREVO_SMTP_KEY not set");

            // ✅ Correct initialization for Brevo SDK
            var config = new brevo_csharp.Client.Configuration();
            config.ApiKey.Add("api-key", _apiKey);

            _emailApi = new TransactionalEmailsApi(config);
        }

        public async System.Threading.Tasks.Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new SendSmtpEmail(
                to: new System.Collections.Generic.List<SendSmtpEmailTo>
                {
                    new SendSmtpEmailTo(toEmail)
                },
                sender: new SendSmtpEmailSender(_fromEmail, "Bodh Script Club"),
                subject: subject,
                htmlContent: htmlBody
            );

            try
            {
                var result = await _emailApi.SendTransacEmailAsync(email);
                Console.WriteLine($"✅ Email sent successfully: {result.MessageId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending email: {ex.Message}");
            }
        }
    }
}

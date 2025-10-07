using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace BodhScriptClubOfficialUI.GlobalService
{
    public class GlobalService
    {

        public abstract class ServiceDeclareMethods
        {
            public abstract Task<string> PostMethod(object data, string Baseurl, string Urlparameters);
            public abstract Task<string> PostAsync<T>(T data, string baseUrl, string endpoint, IFormFile? file = null);
            public abstract  Task SendEmailAsync(string toEmail, string subject, string body);

            public abstract Task<string> GetMethod(string Baseurl, string Urlparameters);
            public abstract Task<string> UploadFileAsync(IFormFile file, string BaseUrl, string Urlparameters);
            public abstract Task<string> GetByIdMethod(string Baseurl, string Urlparameters,int id);
        }
        public class ServiceImplementation : ServiceDeclareMethods
        {
            private readonly IConfiguration _configuration;

            public ServiceImplementation(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public override async Task<string> UploadFileAsync(IFormFile file, string BaseUrl, string Urlparameters)
            {
                if (file == null || file.Length == 0)
                    return string.Empty;

                try
                {
                    using (var client = new HttpClient())
                    using (var content = new MultipartFormDataContent())
                    {
                        // Add file
                        using (var stream = file.OpenReadStream())
                        {
                            var fileContent = new StreamContent(stream);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                            content.Add(fileContent, "file", file.FileName);
                        }

                        string fullUrl = $"{BaseUrl.TrimEnd('/')}/{Urlparameters.TrimStart('/')}";
                        var response = await client.PostAsync(fullUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            var json = System.Text.Json.JsonDocument.Parse(responseContent);
                            return json.RootElement.GetProperty("path").GetString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // log exception if needed
                    return string.Empty;
                }

                return string.Empty;
            }

            public override async Task<string> PostMethod(object data, string Baseurl, string Urlparameters)
            {
                try
                {
                    string jsondata = JsonSerializer.Serialize(data);
                    HttpClient client = new HttpClient();
                    var content = new StringContent(jsondata, Encoding.UTF8, "application/json");
                    string fullUrl = $"{Baseurl.TrimEnd('/')}/{Urlparameters.TrimStart('/')}";

                    HttpResponseMessage response = await client.PostAsync(fullUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string  responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;

                    }
                }
                catch (Exception ex)
                {

                }
                return "";
            }
            public override async Task<string> PostAsync<T>(T data, string baseUrl, string endpoint, IFormFile? file = null)
            {
                using var client = new HttpClient();
                using var form = new MultipartFormDataContent();

                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.PropertyType == typeof(IFormFile)) continue;

                    var value = prop.GetValue(data)?.ToString() ?? "";
                    form.Add(new StringContent(value), prop.Name); // includes MemberPicture
                }

                if (file != null && file.Length > 0)
                {
                    var fileStream = file.OpenReadStream();  // DO NOT dispose manually
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    form.Add(fileContent, "PictureFile", file.FileName);
                }


                string url = $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";

                var response = await client.PostAsync(url, form);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error: {response.StatusCode} - {responseContent}");

                return responseContent;
            }
            //public override  async Task SendEmailAsync(string toEmail, string subject, string body)
            //{
            //    var fromEmail = "bodhscriptclubofficial@gmail.com";       
            //    var fromPassword = "xqhu emfb srez jach";

            //    using var message = new MailMessage();
            //    message.From = new MailAddress(fromEmail);
            //    message.To.Add(toEmail);
            //    message.Subject = subject;
            //    message.Body = body;
            //    message.IsBodyHtml = true;

            //    using var client = new SmtpClient("smtp.gmail.com", 587)
            //    {
            //        Credentials = new NetworkCredential(fromEmail, fromPassword),
            //        EnableSsl = true  // Must be true for TLS
            //    };

            //    await client.SendMailAsync(message);

            //}

            public override async Task SendEmailAsync(string toEmail, string subject, string body)
            {
                var fromEmail = "bodhscriptclubofficial@gmail.com";
                var brevoApiKey = Environment.GetEnvironmentVariable("SENDINBLUE_API_KEY");

                if (string.IsNullOrEmpty(brevoApiKey))
                    throw new Exception("SENDINBLUE_API_KEY is missing.");

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

                try
                {
                    await client.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    // Log full exception
                    Console.WriteLine($"Mail sending failed: {ex.Message} | {ex.InnerException}");
                    throw;
                }
            }


            public override async Task<string> GetMethod(string Baseurl, string Urlparameters)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string requestUrl = $"{Baseurl.TrimEnd('/')}/{Urlparameters.TrimStart('/')}";
                        HttpResponseMessage response = await client.GetAsync(requestUrl);
                        //HttpResponseMessage response = await client.GetAsync(Baseurl + Urlparameters);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            return responseContent;
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }

                return null;
            }

            public override async Task<string> GetByIdMethod(string Baseurl, string Urlparameters,int id)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string requestUrl = $"{Baseurl.TrimEnd('/')}/{Urlparameters.TrimStart('/')}/{id}";

                        HttpResponseMessage response = await client.GetAsync(requestUrl);


                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            return responseContent;
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }

                return "";
            }

        }

    }
}

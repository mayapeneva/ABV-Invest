namespace ABV_Invest.Common.EmailSender
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Newtonsoft.Json;

    public class EmailSender : IEmailSender
    {
        private const string AuthenticationScheme = "Bearer";
        private const string BaseUrl = "https://api.sendgrid.com/v3/";
        private const string SendEmailUrlPath = "mail/send";
        private const string fromAddress = "maya.peneva@gmail.com";
        private const string fromName = "ABV Invest";

        private readonly HttpClient httpClient;

        public EmailSender()
        {
            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                throw new ArgumentOutOfRangeException(nameof(fromAddress));
            }

            if (string.IsNullOrWhiteSpace(fromName))
            {
                throw new ArgumentOutOfRangeException(nameof(fromName));
            }

            var apiKey = Environment.GetEnvironmentVariable("ABV_Invest_SendGrid");
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(AuthenticationScheme, apiKey);
            this.httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                throw new ArgumentOutOfRangeException(nameof(fromAddress));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentOutOfRangeException(nameof(email));
            }

            if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Subject and/or message must be provided.");
            }

            var msg = new SendGridMessage(
                new SendGridEmail(email),
                subject,
                new SendGridEmail(fromAddress, fromName),
                message);
            try
            {
                var json = JsonConvert.SerializeObject(msg);
                var response = await this.httpClient.PostAsync(
                    SendEmailUrlPath,
                    new StringContent(json, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    // See if we can read the response for more information, then log the error
                    var errorJson = await response.Content.ReadAsStringAsync();
                    throw new Exception(
                        $"SendGrid indicated failure! Code: {response.StatusCode}, reason: {errorJson}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception during sending email: {ex}");
            }
        }
    }
}
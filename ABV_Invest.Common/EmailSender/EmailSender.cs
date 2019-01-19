namespace ABV_Invest.Common.EmailSender
{
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public class EmailSender : IEmailSender
    {
        private const string EnvironmentVariable = "ABV_Invest_SendGrid";
        private const string AuthenticationScheme = "Bearer";
        private const string BaseUrl = "https://api.sendgrid.com/v3/";
        private const string SendEmailUrlPath = "mail/send";
        private const string FromAddress = "maya.peneva@gmail.com";
        private const string FromName = "ABV Invest";

        private readonly HttpClient httpClient;

        public EmailSender()
        {
            if (string.IsNullOrWhiteSpace(FromAddress))
            {
                throw new ArgumentOutOfRangeException(nameof(FromAddress));
            }

            if (string.IsNullOrWhiteSpace(FromName))
            {
                throw new ArgumentOutOfRangeException(nameof(FromName));
            }

            var apiKey = Environment.GetEnvironmentVariable(EnvironmentVariable);
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(AuthenticationScheme, apiKey);
            this.httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(FromAddress))
            {
                throw new ArgumentOutOfRangeException(nameof(FromAddress));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentOutOfRangeException(nameof(email));
            }

            if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(Messages.SubjectMessageToBeProvided);
            }

            var msg = new SendGridMessage(
                new SendGridEmail(email),
                subject,
                new SendGridEmail(FromAddress, FromName),
                message);
            try
            {
                var json = JsonConvert.SerializeObject(msg);
                var response = await this.httpClient.PostAsync(
                    SendEmailUrlPath,
                    new StringContent(json, Encoding.UTF8, Constants.JsonContentType));

                if (!response.IsSuccessStatusCode)
                {
                    // See if we can read the response for more information, then log the error
                    var errorJson = await response.Content.ReadAsStringAsync();
                    throw new Exception(string.Format(Messages.SendGridFailure, response.StatusCode, errorJson));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Messages.SendEmailException, ex));
            }
        }
    }
}
namespace ABV_Invest.Common.EmailSender
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class SendGridPersonalization
    {
        [JsonProperty("to")]
        public List<SendGridEmail> To { get; set; }

        [JsonProperty("bcc")]
        public IEnumerable<SendGridEmail> Bcc { get; set; }

        [JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
        public string Subject { get; set; }
    }
}
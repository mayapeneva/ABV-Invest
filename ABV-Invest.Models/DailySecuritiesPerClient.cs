namespace ABV_Invest.Models
{
    using System;
    using System.Collections.Generic;

    public class DailySecuritiesPerClient : BaseEntity
    {
        public DailySecuritiesPerClient()
        {
            this.SecuritiesPerIssuerCollection = new HashSet<SecuritiesPerClient>();
        }

        public virtual AbvInvestUser AbvInvestUser { get; set; }
        public string AbvInvestUserId { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<SecuritiesPerClient> SecuritiesPerIssuerCollection { get; set; }
    }
}
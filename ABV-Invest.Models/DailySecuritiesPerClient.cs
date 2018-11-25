namespace ABV_Invest.Models
{
    using System;
    using System.Collections.Generic;

    public class DailySecuritiesPerIssuer : BaseEntity
    {
        public DailySecuritiesPerIssuer()
        {
            this.SecuritiesPerIssuerCollection = new HashSet<SecuritiesPerIssuer>();
        }

        public virtual AbvUser Client { get; set; }
        public string ClientId { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<SecuritiesPerIssuer> SecuritiesPerIssuerCollection { get; set; }
    }
}
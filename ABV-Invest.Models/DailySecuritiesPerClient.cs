namespace ABV_Invest.Models
{
    using Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class DailySecuritiesPerClient : BaseEntity<int>
    {
        public DailySecuritiesPerClient()
        {
            this.SecuritiesPerIssuerCollection = new HashSet<SecuritiesPerClient>();
        }

        public virtual AbvInvestUser AbvInvestUser { get; set; }
        public string AbvInvestUserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public virtual ICollection<SecuritiesPerClient> SecuritiesPerIssuerCollection { get; set; }
    }
}
namespace ABV_Invest.Models
{
    using System;
    using System.Collections.Generic;

    public class DailyDeals : BaseEntity
    {
        public DailyDeals()
        {
            this.Deals = new HashSet<Deal>();
        }

        public virtual AbvInvestUser AbvInvestUser { get; set; }
        public string AbvInvestUserId { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<Deal> Deals { get; set; }
    }
}
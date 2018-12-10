namespace ABV_Invest.Models
{
    using System;
    using System.Collections.Generic;
    using Base;

    public class DailyDeals : BaseEntity<int>
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
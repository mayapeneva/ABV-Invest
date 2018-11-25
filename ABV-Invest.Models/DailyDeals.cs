namespace ABV_Invest.Models
{
    using System;
    using System.Collections.Generic;

    public class DailyDeal : BaseEntity
    {
        public DailyDeal()
        {
            this.Deals = new HashSet<Deal>();
        }

        public virtual AbvUser Client { get; set; }
        public string ClientId { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<Deal> Deals { get; set; }
    }
}
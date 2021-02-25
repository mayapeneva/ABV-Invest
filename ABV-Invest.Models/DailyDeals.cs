namespace ABV_Invest.Models
{
    using Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class DailyDeals : BaseEntity<int>
    {
        public DailyDeals()
        {
            this.Deals = new HashSet<Deal>();
        }

        public virtual AbvInvestUser AbvInvestUser { get; set; }
        public string AbvInvestUserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public virtual ICollection<Deal> Deals { get; set; }
    }
}
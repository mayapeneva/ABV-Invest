namespace ABV_Invest.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class DailyBalance
    {
        public virtual AbvInvestUser AbvInvestUser { get; set; }
        public string AbvInvestUserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public virtual Balance Balance { get; set; }
        public int BalanceId { get; set; }
    }
}
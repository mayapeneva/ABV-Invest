namespace ABV_Invest.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Base;

    public class Balance : BaseEntity<int>
    {
        public virtual DailyBalance DaiyBalance { get; set; }
        public int BalanceId { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Cash { get; set; }

        [NotMapped]
        public ICollection<SecuritiesPerClient> UsersLatestPortfolio => this.DaiyBalance.AbvInvestUser.Portfolio.OrderByDescending(p => p.Date)
            .First().SecuritiesPerIssuerCollection;

        [NotMapped]
        public decimal AllSecuritiesAveragePriceBuy => this.UsersLatestPortfolio.Sum(s => s.AveragePriceBuy);

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal AllSecuritiesMarketPrice => this.UsersLatestPortfolio.Sum(s => s.TotalPriceBuy);

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal VirtualProfit => this.UsersLatestPortfolio.Sum(s => s.ProfitPercentаge);

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal VirtualProfitPercentage => this.VirtualProfit * 100 / this.AllSecuritiesAveragePriceBuy;
    }
}
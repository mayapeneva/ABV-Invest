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

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cash { get; set; }

        [NotMapped]
        public ICollection<SecuritiesPerClient> UsersLatestPortfolio { get; private set; }

        [NotMapped]
        public decimal AllSecuritiesAveragePriceBuy { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal AllSecuritiesMarketPrice { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal VirtualProfit { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal VirtualProfitPercentage { get; private set; }

        public void SetBalanceFigures()
        {
            this.UsersLatestPortfolio = this.DaiyBalance.AbvInvestUser.Portfolio.OrderByDescending(p => p.Date)
                .First().SecuritiesPerIssuerCollection;
            this.AllSecuritiesAveragePriceBuy = this.UsersLatestPortfolio.Sum(s => s.AveragePriceBuy);
            this.AllSecuritiesMarketPrice = this.UsersLatestPortfolio.Sum(s => s.TotalPriceBuy);
            this.VirtualProfit = this.UsersLatestPortfolio.Sum(s => s.ProfitPercentаge);
            this.VirtualProfitPercentage = this.VirtualProfit * 100 / this.AllSecuritiesAveragePriceBuy;
        }
    }
}
namespace ABV_Invest.Models
{
    using Base;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Balance : BaseEntity<int>
    {
        private const string DateTimeParseFormat = "dd/MM/yyyy";

        public Balance()
        {
            this.UsersPortfolio = new HashSet<SecuritiesPerClient>();
        }

        public virtual DailyBalance DaiyBalance { get; set; }

        public string CurrencyCode { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cash { get; set; }

        [NotMapped]
        public ICollection<SecuritiesPerClient> UsersPortfolio { get; private set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal AllSecuritiesTotalPriceBuy { get; private set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal AllSecuritiesTotalMarketPrice { get; private set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal VirtualProfit { get; private set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal VirtualProfitPercentage { get; private set; }

        public void SetBalanceFigures(AbvInvestUser user, DateTime date)
        {
            this.UsersPortfolio = user.Portfolio.SingleOrDefault(p => p.Date.ToString(DateTimeParseFormat) == date.ToString(DateTimeParseFormat))?.SecuritiesPerIssuerCollection;

            if (this.UsersPortfolio != null)
            {
                this.AllSecuritiesTotalPriceBuy = this.UsersPortfolio.Sum(s => s.TotalPriceBuy);
                this.AllSecuritiesTotalMarketPrice = this.UsersPortfolio.Sum(s => s.TotalMarketPrice);

                this.VirtualProfit = this.UsersPortfolio.Sum(s => s.ProfitInBGN);
                this.VirtualProfitPercentage = (this.VirtualProfit * 100) / (this.AllSecuritiesTotalPriceBuy + this.Cash);

                this.CurrencyCode = "BGN";
            }
        }
    }
}
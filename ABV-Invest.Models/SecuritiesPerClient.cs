namespace ABV_Invest.Models
{
    using Base;

    public class SecuritiesPerClient : BaseEntity<int>
    {
        public virtual DailySecuritiesPerClient DailySecuritiesPerClient { get; set; }
        public int DailySecuritiesPerClientId { get; set; }

        public virtual Security Security { get; set; }
        public int SecurityId { get; set; }

        public decimal Quantity { get; set; }

        public decimal AveragePriceBuy { get; set; }

        public decimal TotalPriceBuy => this.Quantity * this.AveragePriceBuy;

        public decimal MarketPrice { get; set; }

        public decimal TotalMarketPrice { get; set; }

        public decimal Profit { get; set; }

        public decimal ProfitInBGN { get; set; }

        public decimal ProfitPercentаge { get; set; }

        public decimal PortfolioShare { get; set; }
    }
}
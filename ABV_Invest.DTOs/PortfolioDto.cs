namespace ABV_Invest.DTOs
{
    public class PortfolioDto
    {
        public string SecurityIssuer { get; set; }

        public string SecurityBfbCode { get; set; }

        public int Quantity { get; set; }

        public decimal AveragePriceBuy { get; set; }

        public decimal TotalPriceBuy { get; set; }

        public decimal MarketPrice { get; set; }

        public decimal TotalMarketPrice { get; set; }

        public decimal Profit { get; set; }

        public decimal ProfitPercentаge { get; set; }

        public decimal PortfolioShare { get; set; }
    }
}
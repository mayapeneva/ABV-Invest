namespace ABV_Invest.ViewModels
{
    using DTOs;
    using Mapping.Contracts;

    public class PortfolioViewModel : IMapFrom<PortfolioDto>
    {
        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public string SecurityIsin { get; set; }

        public int Quantity { get; set; }

        public string AveragePriceBuy { get; set; }

        public decimal TotalPriceBuy { get; set; }

        public decimal MarketPrice { get; set; }

        public decimal TotalMarketPrice { get; set; }

        public decimal Profit { get; set; }

        public decimal ProfitPercentаge { get; set; }

        public decimal PortfolioShare { get; set; }
    }
}
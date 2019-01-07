namespace ABV_Invest.DTOs
{
    using System;
    using Mapping.Contracts;
    using Models;

    public class PortfolioDto : IMapFrom<SecuritiesPerClient>
    {
        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public string SecurityIsin { get; set; }

        public DateTime DailySecuritiesPerClientDate { get; set; }

        public int Quantity { get; set; }

        public string CurrencyCode { get; set; }

        public decimal AveragePriceBuy { get; set; }

        public decimal TotalPriceBuy { get; set; }

        public decimal MarketPrice { get; set; }

        public decimal TotalMarketPrice { get; set; }

        public decimal Profit { get; set; }

        public decimal ProfitPercentаge { get; set; }

        public decimal PortfolioShare { get; set; }
    }
}
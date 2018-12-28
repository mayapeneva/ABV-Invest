namespace ABV_Invest.DTOs
{
    using Mapping.Contracts;
    using Models;

    public class BalanceDto : IMapFrom<Balance>
    {
        public string CurrencyCode { get; set; }

        public decimal Cash { get; set; }

        public decimal AllSecuritiesTotalMarketPrice { get; set; }

        public decimal VirtualProfit { get; set; }

        public decimal VirtualProfitPercentage { get; set; }
    }
}
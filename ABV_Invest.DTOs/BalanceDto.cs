namespace ABV_Invest.DTOs
{
    using Mapping.Contracts;
    using Models;

    public class BalanceDto : IMapFrom<Balance>
    {
        public decimal Cash { get; set; }

        public decimal AllSecuritiesMarketPrice { get; set; }

        public decimal VirtualProfit { get; set; }

        public decimal VirtualProfitPercentage { get; set; }
    }
}
namespace ABV_Invest.Models
{
    using Base;

    public class Balance : BaseEntity<int>
    {
        public virtual AbvInvestUser AbvInvestUser { get; set; }
        public string AbvInvestUserId { get; set; }

        public decimal MoneyInvested { get; set; }

        public decimal Cash { get; set; }

        public decimal TotalSecuritiesMarketPrice { get; set; }

        public decimal ActualProfit { get; set; }

        public decimal ActualProfitPercentage { get; set; }

        public decimal PossibleProfit { get; set; }
    }
}
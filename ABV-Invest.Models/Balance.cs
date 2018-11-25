namespace ABV_Invest.Models
{
    public class Balance : BaseEntity
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
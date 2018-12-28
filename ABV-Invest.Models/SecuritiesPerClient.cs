namespace ABV_Invest.Models
{
    using System.ComponentModel.DataAnnotations;
    using Base;

    public class SecuritiesPerClient : BaseEntity<int>
    {
        public virtual DailySecuritiesPerClient DailySecuritiesPerClient { get; set; }
        public int DailySecuritiesPerClientId { get; set; }

        public virtual Security Security { get; set; }
        public int SecurityId { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Quantity { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal AveragePriceBuy { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal TotalPriceBuy => this.Quantity * this.AveragePriceBuy;

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal MarketPrice { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal TotalMarketPrice { get; set; }

        [Required]
        [Range(typeof(decimal), "-79228162514264337593543950335", "79228162514264337593543950335")]
        public decimal Profit { get; set; }

        [Required]
        [Range(typeof(decimal), "-79228162514264337593543950335", "79228162514264337593543950335")]
        public decimal ProfitInBGN { get; set; }

        [Required]
        [Range(typeof(decimal), "-79228162514264337593543950335", "79228162514264337593543950335")]
        public decimal ProfitPercentаge { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal PortfolioShare { get; set; }
    }
}
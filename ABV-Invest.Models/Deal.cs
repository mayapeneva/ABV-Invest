namespace ABV_Invest.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Base;
    using Enums;

    public class Deal : BaseEntity<int>
    {
        public virtual DailyDeals DailyDeals { get; set; }
        public int DailyDealsId { get; set; }

        [Required]
        public DealType DealType { get; set; }

        public virtual Security Security { get; set; }
        public int SecurityId { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Quantity { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Fee { get; set; }

        public virtual Currency Currency { get; set; }
        public int CurrencyId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Settlement { get; set; }

        public virtual Market Market { get; set; }
        public int MarketId { get; set; }
    }
}
namespace ABV_Invest.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
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
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Coupon { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal TotalPriceInBGN { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
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
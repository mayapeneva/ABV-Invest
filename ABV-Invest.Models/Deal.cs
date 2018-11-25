namespace ABV_Invest.Models
{
    using System;
    using Enums;

    public class Deal : BaseEntity
    {
        public DealType DealType { get; set; }

        public virtual Security Security { get; set; }
        public int SecurityId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal Fee { get; set; }

        public virtual Currency Currency { get; set; }
        public int CurrencyId { get; set; }

        public DateTime Settlement { get; set; }

        public Market Market { get; set; }
        public int MarketId { get; set; }
    }
}
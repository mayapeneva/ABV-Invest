namespace ABV_Invest.DTOs
{
    using Mapping.Contracts;
    using Models;
    using Models.Enums;
    using System;

    public class DealDto : IMapFrom<Deal>
    {
        public DealType DealType { get; set; }

        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public DateTime DailyDealsDate { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Coupon { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal Fee { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime Settlement { get; set; }

        public string MarketName { get; set; }
    }
}
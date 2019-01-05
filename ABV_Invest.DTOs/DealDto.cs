namespace ABV_Invest.DTOs
{
    using System;
    using AutoMapper;
    using Mapping.Contracts;
    using Models;
    using Models.Enums;

    public class DealDto : IMapFrom<Deal>
    {
        public DealType DealType { get; set; }

        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal Fee { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime Settlement { get; set; }

        public string MarketName { get; set; }
    }
}
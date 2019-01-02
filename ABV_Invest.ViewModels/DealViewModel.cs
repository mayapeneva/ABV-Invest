namespace ABV_Invest.ViewModels
{
    using DTOs;
    using Mapping.Contracts;

    public class DealViewModel : IMapFrom<DealDto>
    {
        public string DealType { get; set; }

        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public int Quantity { get; set; }

        public string Price { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal Fee { get; set; }

        public string CurrencyCode { get; set; }

        public string Settlement { get; set; }

        public string MarketName { get; set; }
    }
}
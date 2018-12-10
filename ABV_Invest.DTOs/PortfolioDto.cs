namespace ABV_Invest.DTOs
{
    using AutoMapper;
    using Mapping.Contracts;
    using Models;

    public class PortfolioDto : IMapFrom<SecuritiesPerClient>, ICustomMap
    {
        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public string SecurityIsin { get; set; }

        public int Quantity { get; set; }

        public string AveragePriceBuy { get; set; }

        public decimal TotalPriceBuy { get; set; }

        public decimal MarketPrice { get; set; }

        public decimal TotalMarketPrice { get; set; }

        public decimal Profit { get; set; }

        public decimal ProfitPercentаge { get; set; }

        public decimal PortfolioShare { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<SecuritiesPerClient, PortfolioDto>()
                .ForMember(dest => dest.AveragePriceBuy, opt => opt.MapFrom(src => src.AveragePriceBuy.ToString("F3")));
        }
    }
}
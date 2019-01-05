namespace ABV_Invest.ViewModels
{
    using AutoMapper;
    using DTOs;
    using Mapping.Contracts;

    using System.Globalization;

    public class PortfolioViewModel : IMapFrom<PortfolioDto>, ICustomMap
    {
        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public string SecurityIsin { get; set; }

        public string Quantity { get; set; }

        public string CurrencyCode { get; set; }

        public string AveragePriceBuy { get; set; }

        public string TotalPriceBuy { get; set; }

        public string MarketPrice { get; set; }

        public string TotalMarketPrice { get; set; }

        public decimal Profit { get; set; }

        public string ProfitPercentаge { get; set; }

        public string PortfolioShare { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<PortfolioDto, PortfolioViewModel>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity.ToString("N", CultureInfo.CreateSpecificCulture("sv-SE"))))
                .ForMember(dest => dest.AveragePriceBuy, opt => opt.MapFrom(src => src.AveragePriceBuy.ToString("N3", CultureInfo.CreateSpecificCulture("sv-SE"))))
                .ForMember(dest => dest.TotalPriceBuy, opt => opt.MapFrom(src => src.TotalPriceBuy.ToString("N2", CultureInfo.CreateSpecificCulture("sv-SE"))))
                .ForMember(dest => dest.MarketPrice, opt => opt.MapFrom(src => src.MarketPrice.ToString("N3", CultureInfo.CreateSpecificCulture("sv-SE"))))
                .ForMember(dest => dest.TotalMarketPrice, opt => opt.MapFrom(src => src.TotalMarketPrice.ToString("N2", CultureInfo.CreateSpecificCulture("sv-SE"))))
                .ForMember(dest => dest.ProfitPercentаge, opt => opt.MapFrom(src => src.ProfitPercentаge.ToString("N2", CultureInfo.CreateSpecificCulture("sv-SE"))))
                .ForMember(dest => dest.PortfolioShare, opt => opt.MapFrom(src => src.PortfolioShare.ToString("N2", CultureInfo.CreateSpecificCulture("sv-SE"))));
        }
    }
}
namespace ABV_Invest.ViewModels
{
    using System.Globalization;
    using AutoMapper;
    using DTOs;
    using Mapping.Contracts;

    public class BalanceViewModel : IMapFrom<BalanceDto>, ICustomMap
    {
        public string CurrencyCode { get; set; }

        public string Cash { get; set; }

        public string AllSecuritiesTotalPriceBuy { get; set; }

        public string AllSecuritiesTotalMarketPrice { get; set; }

        public string VirtualProfit { get; set; }

        public string VirtualProfitPercentage { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<BalanceDto, BalanceViewModel>()
                .ForMember(dest => dest.Cash,
                    opt => opt.MapFrom(src => src.Cash.ToString("N2", CultureInfo.CreateSpecificCulture("sv-SE"))))
                .ForMember(dest => dest.AllSecuritiesTotalPriceBuy,
                    opt => opt.MapFrom(src => src.AllSecuritiesTotalPriceBuy.ToString("N3", CultureInfo.CreateSpecificCulture("sv-SE"))))
                .ForMember(dest => dest.AllSecuritiesTotalMarketPrice,
                    opt => opt.MapFrom(src => src.AllSecuritiesTotalMarketPrice.ToString("N3", CultureInfo.CreateSpecificCulture("sv-SE"))))
                .ForMember(dest => dest.VirtualProfit,
                    opt => opt.MapFrom(src => src.VirtualProfit.ToString("N2", CultureInfo.CreateSpecificCulture("sv-SE"))))
                .ForMember(dest => dest.VirtualProfitPercentage,
                    opt => opt.MapFrom(src => src.VirtualProfitPercentage.ToString("N2", CultureInfo.CreateSpecificCulture("sv-SE"))));
        }
    }
}
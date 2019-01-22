namespace ABV_Invest.ViewModels
{
    using AutoMapper;
    using DTOs;
    using Mapping.Contracts;

    using System.Globalization;

    public class DealViewModel : IMapFrom<DealDto>, ICustomMap
    {
        public string DealType { get; set; }

        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public string DailyDealsDate { get; set; }

        public string Quantity { get; set; }

        public string Price { get; set; }

        public string Coupon { get; set; }

        public string TotalPrice { get; set; }

        public string Fee { get; set; }

        public string CurrencyCode { get; set; }

        public string Settlement { get; set; }

        public string MarketName { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<DealDto, DealViewModel>()
                .ForMember(dest => dest.DealType, opt => opt.MapFrom(src => src.DealType.ToString()))
                .ForMember(dest => dest.DailyDealsDate, opt => opt.MapFrom(src => src.DailyDealsDate.ToString(ViewModelConstants.DateTimeParseFormat)))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity.ToString("N", CultureInfo.CreateSpecificCulture(ViewModelConstants.SvSeCulture))))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.ToString("N3", CultureInfo.CreateSpecificCulture(ViewModelConstants.SvSeCulture))))
                .ForMember(dest => dest.Coupon, opt => opt.MapFrom(src => src.Coupon.ToString("N3", CultureInfo.CreateSpecificCulture(ViewModelConstants.SvSeCulture))))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice.ToString("N2", CultureInfo.CreateSpecificCulture(ViewModelConstants.SvSeCulture))))
                .ForMember(dest => dest.Fee, opt => opt.MapFrom(src => src.Fee.ToString("N2", CultureInfo.CreateSpecificCulture(ViewModelConstants.SvSeCulture))))
                .ForMember(dest => dest.Settlement, opt => opt.MapFrom(src => src.Settlement.ToString(ViewModelConstants.DateTimeParseFormat)));
        }
    }
}
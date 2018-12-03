namespace ABV_Invest.Common
{
    using AutoMapper;
    using DTOs;
    using Models;
    using ViewModels;

    public class ABV_InvestProfile : Profile
    {
        public ABV_InvestProfile()
        {
            this.CreateMap<SecuritiesPerClient, PortfolioDto>()
                .ForMember(dest => dest.SecurityIssuer, opt => opt.MapFrom(src => src.Security.Issuer.Name))
                .ForMember(dest => dest.SecurityBfbCode, opt => opt.MapFrom(src => src.Security.BfbCode))
                .ForMember(dest => dest.AveragePriceBuy, opt => opt.MapFrom(src => src.AveragePriceBuy.ToString("F3")));
            this.CreateMap<PortfolioDto, PortfolioViewModel>();

            this.CreateMap<Deal, DealsDto>()
                .ForMember(dest => dest.DealType, opt => opt.MapFrom(src => src.DealType.ToString()))
                .ForMember(dest => dest.SecurityBfbCode, opt => opt.MapFrom(src => src.Security.BfbCode)).ForMember(dest => dest.SecurityIssuer, opt => opt.MapFrom(src => src.Security.Issuer.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.ToString("F3")))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency.Code))
                .ForMember(dest => dest.Settlement, opt => opt.MapFrom(src => src.Settlement.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market.Name));
            this.CreateMap<DealsDto, DealsViewModel>();
        }
    }
}
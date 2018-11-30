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
                .ForMember(dest => dest.SecurityBfbCode, opt => opt.MapFrom(src => src.Security.BfbCode));
            this.CreateMap<PortfolioDto, PortfolioViewModel>();
        }
    }
}
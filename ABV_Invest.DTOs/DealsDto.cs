namespace ABV_Invest.DTOs
{
    using AutoMapper;
    using Mapping.Contracts;
    using Models;

    public class DealsDto : IMapFrom<Deal>, ICustomMap
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

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Deal, DealsDto>()
                .ForMember(dest => dest.DealType, opt => opt.MapFrom(src => src.DealType.ToString()))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.ToString("F3")))
                .ForMember(dest => dest.Settlement, opt => opt.MapFrom(src => src.Settlement.ToString("dd/MM/yyyy")));
        }
    }
}
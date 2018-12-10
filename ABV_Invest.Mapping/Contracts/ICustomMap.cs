namespace ABV_Invest.Mapping.Contracts
{
    using AutoMapper;

    public interface ICustomMap
    {
        void CreateMappings(IMapperConfigurationExpression configuration);
    }
}
namespace ABV_Invest.Common.Mapping.Contracts
{
    using AutoMapper;

    public interface ICustomMap
    {
        void CreateMappings(IMapperConfigurationExpression configuration);
    }
}
namespace ABV_Invest.DTOs
{
    using Mapping.Contracts;
    using Models;

    public class UserDto : IMapFrom<AbvInvestUser>
    {
        public string PIN { get; set; }
    }
}
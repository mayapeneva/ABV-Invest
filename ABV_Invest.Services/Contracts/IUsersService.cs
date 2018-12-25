namespace ABV_Invest.Services.Contracts
{
    using DTOs;

    public interface IUsersService
    {
        T GetUserByUserName<T>(string username);
    }
}
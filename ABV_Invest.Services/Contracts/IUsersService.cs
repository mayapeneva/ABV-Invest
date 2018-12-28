namespace ABV_Invest.Services.Contracts
{
    public interface IUsersService
    {
        T GetUserByUserName<T>(string username);
    }
}
namespace ABV_Invest.Services.Contracts
{
    using DTOs;

    public interface IUsersService
    {
        UserDto GetUserByUserName(string username);
    }
}
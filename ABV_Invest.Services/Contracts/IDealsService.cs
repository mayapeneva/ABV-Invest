namespace ABV_Invest.Services.Contracts
{
    using DTOs;

    public interface IDealsService
    {
        T[] GetUserDailyDeals<T>(string userId, string chosenDate);
    }
}
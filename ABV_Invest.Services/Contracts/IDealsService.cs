namespace ABV_Invest.Services.Contracts
{
    public interface IDealsService
    {
        T[] GetUserDailyDeals<T>(string userId, string chosenDate);
    }
}
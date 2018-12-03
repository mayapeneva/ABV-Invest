namespace ABV_Invest.Services.Contracts
{
    using DTOs;

    public interface IDealsService
    {
        DealsDto[] GetUserDailyDeals(string userId, string chosenDate);
    }
}
namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Threading.Tasks;
    using BindingModels.Uploads.Deals;

    public interface IDealsService
    {
        T[] GetUserDailyDeals<T>(string userId, string chosenDate);

        Task<bool> SeedDeals(DealRowBindingModel[] deserializedDeals, DateTime date);
    }
}
namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Threading.Tasks;
    using BindingModels.Uploads.Deals;
    using Models;

    public interface IDealsService
    {
        T[] GetUserDailyDeals<T>(AbvInvestUser user, string chosenDate);

        Task<bool> SeedDeals(DealRowBindingModel[] deserializedDeals, DateTime date);
    }
}
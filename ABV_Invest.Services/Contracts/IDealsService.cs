namespace ABV_Invest.Services.Contracts
{
    using BindingModels.Uploads.Deals;

    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IDealsService
    {
        T[] GetUserDailyDeals<T>(ClaimsPrincipal user, string chosenDate);

        Task<bool> SeedDeals(DealRowBindingModel[] deserializedDeals, DateTime date);
    }
}
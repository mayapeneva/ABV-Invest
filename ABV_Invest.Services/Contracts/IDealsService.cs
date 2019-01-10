namespace ABV_Invest.Services.Contracts
{
    using BindingModels.Uploads.Deals;

    using System;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDealsService
    {
        T[] GetUserDailyDeals<T>(ClaimsPrincipal user, DateTime date);

        Task<StringBuilder> SeedDeals(DealRowBindingModel[] deserializedDeals, DateTime date);
    }
}
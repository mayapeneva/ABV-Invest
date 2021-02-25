namespace ABV_Invest.Services.Contracts
{
    using ABV_Invest.Common.BindingModels.Uploads.Deals;
    using System;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDealsService
    {
        Task<T[]> GetUserDailyDeals<T>(ClaimsPrincipal user, DateTime date);

        Task<StringBuilder> SeedDeals(DealRowBindingModel[] deserializedDeals, DateTime date);
    }
}
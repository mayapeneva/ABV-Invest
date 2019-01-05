namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using BindingModels.Uploads.Portfolios;

    public interface IPortfoliosService
    {
        T[] GetUserDailyPortfolio<T>(ClaimsPrincipal user, string chosenDate);

        Task<bool> SeedPortfolios(IEnumerable<PortfolioRowBindingModel> deserializedPortfolios, DateTime date);
    }
}
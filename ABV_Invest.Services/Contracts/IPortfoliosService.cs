namespace ABV_Invest.Services.Contracts
{
    using BindingModels.Uploads.Portfolios;

    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IPortfoliosService
    {
        T[] GetUserDailyPortfolio<T>(ClaimsPrincipal user, string chosenDate);

        Task<bool> SeedPortfolios(IEnumerable<PortfolioRowBindingModel> deserializedPortfolios, DateTime date);
    }
}
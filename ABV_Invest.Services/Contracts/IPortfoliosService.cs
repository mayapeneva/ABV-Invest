namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BindingModels.Uploads.Portfolios;
    using Models;

    public interface IPortfoliosService
    {
        T[] GetUserDailyPortfolio<T>(AbvInvestUser user, string chosenDate);

        Task<bool> SeedPortfolios(IEnumerable<PortfolioRowBindingModel> deserializedPortfolios, DateTime date);
    }
}
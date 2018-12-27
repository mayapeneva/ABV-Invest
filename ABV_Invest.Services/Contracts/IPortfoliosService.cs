namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BindingModels.Uploads.Portfolios;

    public interface IPortfoliosService
    {
        T[] GetUserDailyPortfolio<T>(string userId, string chosenDate);

        Task<bool> SeedPortfolios(IEnumerable<PortfolioRowBindingModel> objPortfolios, DateTime date);
    }
}
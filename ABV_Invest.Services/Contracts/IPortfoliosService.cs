namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using BindingModels.Uploads.Portfolios;
    using DTOs;

    public interface IPortfoliosService
    {
        T[] GetUserDailyPortfolio<T>(string userId, string chosenDate);

        bool SeedPortfolios(PortfolioRowBindingModel[] objPortfolios);
    }
}
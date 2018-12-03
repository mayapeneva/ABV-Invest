namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using DTOs;

    public interface IPortfoliosService
    {
        PortfolioDto[] GetUserDailyPortfolio(string userId, string chosenDate);
    }
}
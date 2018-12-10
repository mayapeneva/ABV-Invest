namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using DTOs;

    public interface IPortfoliosService
    {
        T[] GetUserDailyPortfolio<T>(string userId, string chosenDate);
    }
}
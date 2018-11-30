namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using DTOs;

    public interface IPortfoliosService
    {
        PortfolioDto[] GetUserPortfolio(string userId, string chosenDate);
    }
}
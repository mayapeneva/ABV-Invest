namespace ABV_Invest.Services.Contracts
{
    using System;
    using Models;

    public interface IBalancesService
    {
        void CreateBalanceForUser(AbvInvestUser user, DateTime date);

        T GetUserDailyBalance<T>(string userId, string chosenDate);
    }
}
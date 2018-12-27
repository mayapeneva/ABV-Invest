namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Threading.Tasks;
    using Models;

    public interface IBalancesService
    {
        Task<bool> CreateBalanceForUser(AbvInvestUser user, DateTime date);

        T GetUserDailyBalance<T>(string userId, string chosenDate);
    }
}
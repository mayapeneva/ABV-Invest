namespace ABV_Invest.Services.Contracts
{
    using System;
    using System.Threading.Tasks;
    using Models;

    public interface IBalancesService
    {
        Task CreateBalanceForUser(AbvInvestUser user, DateTime date);

        T GetUserDailyBalance<T>(string userId, string chosenDate);
    }
}
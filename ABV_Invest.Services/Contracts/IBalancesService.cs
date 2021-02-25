namespace ABV_Invest.Services.Contracts
{
    using Models;
    using System;
    using System.Threading.Tasks;

    public interface IBalancesService
    {
        Task<bool> CreateBalanceForUser(AbvInvestUser user, DateTime date);

        T GetUserDailyBalance<T>(AbvInvestUser user, DateTime date);
    }
}
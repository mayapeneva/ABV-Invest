namespace ABV_Invest.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Base;
    using Contracts;
    using Data;
    using Models;

    public class BalancesService : BaseService, IBalancesService
    {
        public BalancesService(AbvDbContext db)
        : base(db)
        {
        }

        public async Task CreateBalanceForUser(AbvInvestUser user, DateTime date)
        {
            var dailyBalance = new DailyBalance
            {
                Date = date,
                Balance = new Balance()
            };

            user.Balances.Add(dailyBalance);
            await this.Db.SaveChangesAsync();

            dailyBalance.Balance.SetBalanceFigures(user, date);
            await this.Db.SaveChangesAsync();
        }

        public T GetUserDailyBalance<T>(AbvInvestUser user, DateTime date)
        {
            var dailyBalance = user.Balances.SingleOrDefault(b => b.Date == date);
            if (dailyBalance == null)
            {
                return default(T);
            }

            return Mapper.Map<T>(dailyBalance.Balance);
        }
    }
}
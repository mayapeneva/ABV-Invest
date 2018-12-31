namespace ABV_Invest.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Base;
    using Common;
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
            if (!DataValidator.IsValid(dailyBalance))
            {
                return;
            }

            user.Balances.Add(dailyBalance);
            await this.Db.SaveChangesAsync();

            dailyBalance.Balance.SetBalanceFigures(date);
            await this.Db.SaveChangesAsync();
        }

        public T GetUserDailyBalance<T>(string userId, string chosenDate)
        {
            var ifParsed = DateTime.TryParse(chosenDate, out DateTime date);
            if (!ifParsed)
            {
                return default(T);
            }

            var balance = this.Db.Balances.SingleOrDefault(b =>
                b.DaiyBalance.AbvInvestUserId == userId && b.DaiyBalance.Date == date);
            if (balance == null)
            {
                return default(T);
            }

            return Mapper.Map<T>(balance);
        }
    }
}
namespace ABV_Invest.Services
{
    using System;
    using System.Linq;
    using AutoMapper;
    using Contracts;
    using Data;
    using Models;

    public class BalancesService : IBalancesService
    {
        private readonly AbvDbContext db;

        public BalancesService(AbvDbContext db)
        {
            this.db = db;
        }

        public async void CreateBalanceForUser(AbvInvestUser user, DateTime date)
        {
            var balance = new DailyBalance
            {
                Date = date,
                Balance = new Balance()
            };

            user.Balances.Add(balance);
            await this.db.SaveChangesAsync();
        }

        public T GetUserDailyBalance<T>(string userId, string chosenDate)
        {
            var ifParsed = DateTime.TryParse(chosenDate, out DateTime date);
            if (!ifParsed)
            {
                return default(T);
            }

            var balance = this.db.Balances.SingleOrDefault(b =>
                b.DaiyBalance.AbvInvestUserId == userId && b.DaiyBalance.Date == date);

            return Mapper.Map<T>(balance);
        }
    }
}
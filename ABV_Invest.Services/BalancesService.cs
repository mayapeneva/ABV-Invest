﻿namespace ABV_Invest.Services
{
    using System;
    using System.Linq;
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

        public async void CreateBalanceForUser(AbvInvestUser user, DateTime date)
        {
            var balance = new DailyBalance
            {
                Date = date,
                Balance = new Balance()
            };

            if (!IsValid(balance))
            {
                return;
            }

            user.Balances.Add(balance);
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

            return Mapper.Map<T>(balance);
        }
    }
}
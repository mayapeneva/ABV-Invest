namespace ABV_Invest.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Contracts;
    using Data;
    using DTOs;
    using Mapper = AutoMapper.Mapper;

    public class PortfoliosService : IPortfoliosService
    {
        private readonly AbvDbContext db;

        public PortfoliosService(AbvDbContext db)
        {
            this.db = db;
        }

        public T[] GetUserDailyPortfolio<T>(string userId, string chosenDate)
        {
            var ifParsed = DateTime.TryParse(chosenDate, out DateTime date);
            if (!ifParsed)
            {
                return null;
            }

            var portfolio = this.db.DailySecuritiesPerClient.SingleOrDefault(p =>
                p.AbvInvestUserId == userId && p.Date == date);

            var collection = portfolio?.SecuritiesPerIssuerCollection.Select(Mapper.Map<T>).ToArray();

            return collection;
        }
    }
}
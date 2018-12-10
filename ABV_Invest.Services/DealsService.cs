namespace ABV_Invest.Services
{
    using System;
    using System.Linq;
    using AutoMapper;
    using Contracts;
    using Data;
    using DTOs;

    public class DealsService : IDealsService
    {
        private readonly AbvDbContext db;

        public DealsService(AbvDbContext db)
        {
            this.db = db;
        }

        public DealsDto[] GetUserDailyDeals(string userId, string chosenDate)
        {
            var ifParsed = DateTime.TryParse(chosenDate, out DateTime date);
            if (!ifParsed)
            {
                return null;
            }

            var deals = this.db.DailyDeals.SingleOrDefault(p =>
                p.AbvInvestUserId == userId && p.Date == date);

            var collection = deals?.Deals.Select(Mapper.Map<DealsDto>).ToArray();

            return collection;
        }
    }
}
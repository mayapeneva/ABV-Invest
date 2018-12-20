namespace ABV_Invest.Services
{
    using System;
    using System.Linq;
    using AutoMapper;
    using Base;
    using Contracts;
    using Data;

    public class DealsService : BaseService, IDealsService
    {
        public DealsService(AbvDbContext db)
            : base(db)
        {
        }

        public T[] GetUserDailyDeals<T>(string userId, string chosenDate)
        {
            var ifParsed = DateTime.TryParse(chosenDate, out DateTime date);
            if (!ifParsed)
            {
                return null;
            }

            var deals = this.Db.DailyDeals.SingleOrDefault(p =>
                p.AbvInvestUserId == userId && p.Date == date);

            var collection = deals?.Deals.Select(Mapper.Map<T>).ToArray();

            return collection;
        }
    }
}
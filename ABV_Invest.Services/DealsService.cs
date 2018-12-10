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
        private readonly IMapper mapper;

        public DealsService(AbvDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
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

            var collection = deals?.Deals.Select(d => this.mapper.Map<DealsDto>(d)).ToArray();

            return collection;
        }
    }
}
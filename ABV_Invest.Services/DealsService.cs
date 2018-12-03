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
            var date = DateTime.Parse(chosenDate);
            var deals = this.db.DailyDeals.SingleOrDefault(p =>
                p.AbvInvestUserId == userId && p.Date == date);
            if (deals == null)
            {
                return null;
            }

            var collection = deals.Deals.ToArray();
            var collectionCount = deals.Deals.Count;
            var dealsDtos = new DealsDto[collectionCount];
            for (int i = 0; i < collectionCount; i++)
            {
                dealsDtos[i] = this.mapper.Map<DealsDto>(collection[i]);
            }

            return dealsDtos;
        }
    }
}
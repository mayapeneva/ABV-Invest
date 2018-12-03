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
        private readonly IMapper mapper;

        public PortfoliosService(AbvDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public PortfolioDto[] GetUserDailyPortfolio(string userId, string chosenDate)
        {
            var date = DateTime.Parse(chosenDate);
            var portfolio = this.db.DailySecuritiesPerClient.SingleOrDefault(p =>
                p.AbvInvestUserId == userId && p.Date == date);
            if (portfolio == null)
            {
                return null;
            }

            var collection = portfolio.SecuritiesPerIssuerCollection.ToArray();
            var collectionCount = portfolio.SecuritiesPerIssuerCollection.Count;
            var portfolioDtos = new PortfolioDto[collectionCount];
            for (int i = 0; i < collectionCount; i++)
            {
                portfolioDtos[i] = this.mapper.Map<PortfolioDto>(collection[i]);
            }

            return portfolioDtos;
        }
    }
}
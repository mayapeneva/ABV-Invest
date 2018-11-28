namespace ABV_Invest.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Data;
    using DTOs;

    public class PortfoliosService : IPortfoliosService
    {
        private readonly AbvDbContext db;

        public PortfoliosService(AbvDbContext db)
        {
            this.db = db;
        }

        public IEnumerable<PortfolioDto> GetUserPortfolio(string userId, string chosenDate)
        {
            var date = DateTime.Parse(chosenDate);
            var portfolio = this.db.DailySecuritiesPerClient.SingleOrDefault(p =>
                p.AbvInvestUserId == userId && p.Date == date);
            var portfolioDtos = new List<PortfolioDto>();
            foreach (var item in portfolio.SecuritiesPerIssuerCollection)
            {
                portfolioDtos.Add(new PortfolioDto
                {
                    SecurityIssuer = item.Security.Issuer.Name,
                    SecurityBfbCode = item.Security.BfbCode,
                    Quantity = item.Quantity,
                    AveragePriceBuy = item.AveragePriceBuy,
                    TotalPriceBuy = item.TotalPriceBuy,
                    MarketPrice = item.MarketPrice,
                    TotalMarketPrice = item.TotalMarketPrice,
                    Profit = item.Profit,
                    ProfitPercentаge = item.ProfitPercentаge,
                    PortfolioShare = item.PortfolioShare
                });
            }

            return portfolioDtos;
        }
    }
}
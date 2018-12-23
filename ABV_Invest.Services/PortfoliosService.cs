namespace ABV_Invest.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Base;
    using BindingModels.Uploads.Portfolios;
    using Common;
    using Contracts;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Models;
    using Mapper = AutoMapper.Mapper;

    public class PortfoliosService : BaseService, IPortfoliosService
    {
        private const string initialPass = "789-Asd";
        private const string initialPIN = "00001";
        private const string initialEmail = "client@abv.bg";

        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IBalancesService balancesService;

        public PortfoliosService(AbvDbContext db, UserManager<AbvInvestUser> userManager, IBalancesService balancesService)
            : base(db)
        {
            this.userManager = userManager;
            this.balancesService = balancesService;
        }

        public T[] GetUserDailyPortfolio<T>(string userId, string chosenDate)
        {
            var ifParsed = DateTime.TryParse(chosenDate, out DateTime date);
            if (!ifParsed)
            {
                return null;
            }

            var portfolio = this.Db.DailySecuritiesPerClient.SingleOrDefault(p =>
                p.AbvInvestUserId == userId && p.Date == date);

            var collection = portfolio?.SecuritiesPerIssuerCollection.Select(Mapper.Map<T>).ToArray();

            return collection;
        }

        public async Task SeedPortfolios(IEnumerable<PortfolioRowBindingModel> objPortfolios, DateTime date)
        {
            // Group the entries by Client and process portfolios for each client
            var portfolios = objPortfolios.GroupBy(p => p.Client.CDNNumber);
            foreach (var portfolio in portfolios)
            {
                // Check if User exists
                var user = this.Db.AbvInvestUsers.SingleOrDefault(u => u.UserName == portfolio.Key);
                if (user == null)
                {
                    continue;
                }

                // Check if there is a DailySecuritiesEntity created for this User and date already
                if (this.Db.DailySecuritiesPerClient.Any(ds => ds.AbvInvestUserId == user.Id && ds.Date == date))
                {
                    continue;
                }

                // Create new DailySecuritiesEntity
                var dbPortfolio = new DailySecuritiesPerClient
                {
                    Date = date,
                    SecuritiesPerIssuerCollection = new HashSet<SecuritiesPerClient>()
                };

                // Create all SecuritiesPerClient for this User
                foreach (var portfolioRow in portfolio)
                {
                    // Check if such security exists
                    var security =
                        this.Db.Securities.SingleOrDefault(s => s.BfbCode == portfolioRow.Instrument.NewCode);
                    if (security == null)
                    {
                        continue;
                    }

                    // Create the SecuritiesPerClient and add it to the DailySecuritiesPerClient
                    var securitiesPerClient = new SecuritiesPerClient
                    {
                        Security = security,
                        Quantity = portfolioRow.AccountData.Quantity,
                        AveragePriceBuy = portfolioRow.AccountData.OpenPrice,
                        MarketPrice = portfolioRow.AccountData.MarketPrice,
                        TotalMarketPrice = portfolioRow.AccountData.MarketValue,
                        Profit = portfolioRow.AccountData.Result,
                        ProfitInBGN = portfolioRow.AccountData.ResultBGN,
                        ProfitPercentаge = portfolioRow.Other.YieldPercent,
                        PortfolioShare = portfolioRow.Other.RelativePart
                    };
                    if (!IsValid(securitiesPerClient))
                    {
                        continue;
                    }

                    dbPortfolio.SecuritiesPerIssuerCollection.Add(securitiesPerClient);
                    if (!IsValid(dbPortfolio))
                    {
                        continue;
                    }
                }

                user.Portfolio.Add(dbPortfolio);

                await this.Db.SaveChangesAsync();

                this.balancesService.CreateBalanceForUser(user, date);
            }
        }
    }
}
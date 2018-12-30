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

        public async Task<bool> SeedPortfolios(IEnumerable<PortfolioRowBindingModel> deserializedPortfolios, DateTime date)
        {
            var changesCounter = 0;

            // Group the entries by Client and process portfolios for each client
            var portfolios = deserializedPortfolios.GroupBy(p => p.Client.CDNNumber);
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
                    // Fill in user's FullName if empty
                    if (String.IsNullOrWhiteSpace(user.FullName))
                    {
                        user.FullName = portfolioRow.Client.Name;
                    }

                    // Check if such security exists
                    var security =
                        this.Db.Securities.SingleOrDefault(s => s.ISIN == portfolioRow.Instrument.ISIN);
                    if (security == null)
                    {
                        continue;
                    }

                    // Create the SecuritiesPerClient and add it to the DailySecuritiesPerClient
                    var quantity = decimal.Parse(portfolioRow.AccountData.Quantity.Replace(" ", ""));
                    var currency = this.Db.Currencies.SingleOrDefault(c => c.Code == portfolioRow.Instrument.Currency);
                    if (currency == null)
                    {
                        continue;
                    }

                    var averagePriceBuy = decimal.Parse(portfolioRow.AccountData.OpenPrice.Replace(" ", ""));
                    var marketPrice = decimal.Parse(portfolioRow.AccountData.MarketPrice.Replace(" ", ""));
                    var totalMarketPrice = decimal.Parse(portfolioRow.AccountData.MarketValue.Replace(" ", ""));
                    var profit = decimal.Parse(portfolioRow.AccountData.Result.Replace(" ", ""));
                    var profitInBGN = decimal.Parse(portfolioRow.AccountData.ResultBGN.Replace(" ", ""));
                    var profitPercent = decimal.Parse(portfolioRow.Other.YieldPercent.Replace(" ", ""));
                    var portfolioShare = decimal.Parse(portfolioRow.Other.RelativePart.Replace(" ", ""));
                    var securitiesPerClient = new SecuritiesPerClient
                    {
                        Security = security,
                        Quantity = quantity,
                        Currency = currency,
                        AveragePriceBuy = averagePriceBuy,
                        MarketPrice = marketPrice,
                        TotalMarketPrice = totalMarketPrice,
                        Profit = profit,
                        ProfitInBGN = profitInBGN,
                        ProfitPercentаge = profitPercent,
                        PortfolioShare = portfolioShare
                    };

                    // Validate SecuritiesPerClient and add them to the portfolio
                    if (!DataValidator.IsValid(securitiesPerClient))
                    {
                        continue;
                    }
                    dbPortfolio.SecuritiesPerIssuerCollection.Add(securitiesPerClient);
                }

                // Validate portfolio and add it to user's Portfolios
                if (!DataValidator.IsValid(dbPortfolio))
                {
                    continue;
                }
                user.Portfolio.Add(dbPortfolio);
                var result = await this.Db.SaveChangesAsync();

                // Creating balance for this user and this date
                if (result > 0)
                {
                    await this.balancesService.CreateBalanceForUser(user, date);

                    changesCounter += result;
                }
            }

            if (changesCounter == 0)
            {
                return false;
            }

            return true;
        }
    }
}
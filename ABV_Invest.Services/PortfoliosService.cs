namespace ABV_Invest.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BindingModels.Uploads.Portfolios;
    using Contracts;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Models;
    using Mapper = AutoMapper.Mapper;

    public class PortfoliosService : IPortfoliosService
    {
        private const string initialPass = "789-Asd";

        private readonly AbvDbContext db;
        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IBalancesService balancesService;

        public PortfoliosService(AbvDbContext db, UserManager<AbvInvestUser> userManager, IBalancesService balancesService)
        {
            this.db = db;
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

            var portfolio = this.db.DailySecuritiesPerClient.SingleOrDefault(p =>
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
                // Check if User exists and if not create new User
                var user = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == portfolio.Key);
                if (user == null)
                {
                    var result = this.userManager.CreateAsync(new AbvInvestUser
                    {
                        UserName = portfolio.Key,
                    }).Result;

                    if (result.Succeeded)
                    {
                        user = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == portfolio.Key);
                        await this.userManager.HasPasswordAsync(user);
                    }
                    else
                    {
                        continue;
                    }
                }

                // Check if there is a DailySecuritiesEntity created for this User and date already
                if (this.db.DailySecuritiesPerClient.Any(ds => ds.AbvInvestUserId == user.Id && ds.Date == date))
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
                    if (user.FullName == string.Empty)
                    {
                        user.FullName = portfolioRow.Client.Name;
                    }

                    // Check if such security exists and if not - create new one
                    var security =
                        this.db.Securities.SingleOrDefault(s => s.BfbCode == portfolioRow.Instrument.NewCode);
                    if (security == null)
                    {
                        // Check if issuer exists and if not - create new one
                        var issuer = this.db.Issuers.SingleOrDefault(i =>
                            i.Securities.Any(s => s.ISIN == portfolioRow.Instrument.ISIN));
                        if (issuer == null)
                        {
                            issuer = new Issuer
                            {
                                Name = portfolioRow.Instrument.Issuer,
                                Securities = new HashSet<Security>()
                            };
                            await this.db.Issuers.AddAsync(issuer);

                            // Check if currency exists and if not - create new one
                            var currency =
                                this.db.Currencies.SingleOrDefault(c => c.Code == portfolioRow.Instrument.Currency);
                            if (currency == null)
                            {
                                currency = new Currency
                                {
                                    Code = portfolioRow.Instrument.Currency
                                };
                                await this.db.Currencies.AddAsync(currency);
                            }

                            security = new Security
                            {
                                Issuer = issuer,
                                ISIN = portfolioRow.Instrument.ISIN,
                                BfbCode = portfolioRow.Instrument.NewCode,
                                Currency = currency
                            };

                            if (issuer.Securities.All(s => s.ISIN != security.ISIN))
                            {
                                issuer.Securities.Add(security);
                            }
                        }
                    }

                    // Create the SecuritiesPerClient and add it to the DailySecuritiesPerClient
                    dbPortfolio.SecuritiesPerIssuerCollection.Add(new SecuritiesPerClient
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
                    });

                    await this.db.SaveChangesAsync();
                }

                user.Portfolio.Add(dbPortfolio);

                await this.db.SaveChangesAsync();

                this.balancesService.CreateBalanceForUser(user, date);
            }
        }
    }
}
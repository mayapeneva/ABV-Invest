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
                // Check if User exists and if not create new User
                var user = this.Db.AbvInvestUsers.SingleOrDefault(u => u.UserName == portfolio.Key);
                if (user == null)
                {
                    user = new AbvInvestUser
                    {
                        UserName = portfolio.Key,
                        PIN = initialPIN,
                        Email = initialEmail,
                        SecurityStamp = Guid.NewGuid().ToString("D")
                    };

                    var result = this.userManager.CreateAsync(user, initialPass).GetAwaiter().GetResult();
                    if (!result.Succeeded)
                    {
                        continue;
                    }

                    result = this.userManager.AddToRoleAsync(user, Constants.User).GetAwaiter().GetResult();
                    if (!result.Succeeded)
                    {
                        continue;
                    }

                    user = this.Db.AbvInvestUsers.SingleOrDefault(u => u.UserName == portfolio.Key);
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
                    if (user.FullName == string.Empty)
                    {
                        user.FullName = portfolioRow.Client.Name;
                    }

                    // Check if such security exists and if not - create new one
                    var security =
                        this.Db.Securities.SingleOrDefault(s => s.BfbCode == portfolioRow.Instrument.NewCode);
                    if (security == null)
                    {
                        // Check if issuer exists and if not - create new one
                        var issuer = this.Db.Issuers.SingleOrDefault(i =>
                            i.Securities.Any(s => s.ISIN == portfolioRow.Instrument.ISIN));
                        if (issuer == null)
                        {
                            issuer = new Issuer
                            {
                                Name = portfolioRow.Instrument.Issuer,
                                Securities = new HashSet<Security>()
                            };

                            if (!IsValid(issuer))
                            {
                                continue;
                            }

                            this.Db.Issuers.AddAsync(issuer).GetAwaiter().GetResult();

                            // Check if currency exists and if not - create new one
                            var currency =
                                this.Db.Currencies.SingleOrDefault(c => c.Code == portfolioRow.Instrument.Currency);
                            if (currency == null)
                            {
                                currency = new Currency
                                {
                                    Code = portfolioRow.Instrument.Currency
                                };

                                if (!IsValid(currency))
                                {
                                    continue;
                                }

                                this.Db.Currencies.AddAsync(currency).GetAwaiter().GetResult();
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

                    if (!IsValid(dbPortfolio))
                    {
                        continue;
                    }
                }

                user.Portfolio.Add(dbPortfolio);

                var result2 = this.Db.SaveChangesAsync().GetAwaiter().GetResult();
                if (result2 == 0)
                {
                    continue;
                }
                this.balancesService.CreateBalanceForUser(user, date);
            }
        }
    }
}
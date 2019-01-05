namespace ABV_Invest.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
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
        private readonly IDataService dataService;

        public PortfoliosService(AbvDbContext db, UserManager<AbvInvestUser> userManager, IBalancesService balancesService, IDataService dataService) : base(db)
        {
            this.userManager = userManager;
            this.balancesService = balancesService;
            this.dataService = dataService;
        }

        public T[] GetUserDailyPortfolio<T>(ClaimsPrincipal user, string chosenDate)
        {
            var date = DateTime.Parse(chosenDate);
            var dbUser = this.userManager.GetUserAsync(user).GetAwaiter().GetResult();
            return dbUser?.Portfolio
                .SingleOrDefault(p => p.Date == date)?
                .SecuritiesPerIssuerCollection
                .Select(Mapper.Map<T>)
                .ToArray();
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

                // Create new DailySecuritiesEntity
                var dbPortfolio = new DailySecuritiesPerClient
                {
                    Date = date,
                    SecuritiesPerIssuerCollection = new HashSet<SecuritiesPerClient>()
                };

                // Check if there is a DailySecuritiesEntity created for this User and date already
                if (this.Db.DailySecuritiesPerClient.Any(ds => ds.AbvInvestUserId == user.Id && ds.Date == date))
                {
                    dbPortfolio = this.Db.DailySecuritiesPerClient.Single(ds => ds.AbvInvestUserId == user.Id && ds.Date == date);
                }

                // Create all SecuritiesPerClient for this User
                foreach (var portfolioRow in portfolio)
                {
                    // Fill in user's FullName if empty
                    if (String.IsNullOrWhiteSpace(user.FullName))
                    {
                        user.FullName = portfolioRow.Client.Name;
                    }

                    // Check if such security exists and if not create it
                    var security =
                        this.Db.Securities.SingleOrDefault(s => s.ISIN == portfolioRow.Instrument.ISIN);
                    if (security == null)
                    {
                        var securityInfo = portfolioRow.Instrument;
                        var securityResult = this.dataService.CreateSecurity(securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode,
                            securityInfo.Currency);
                        if (!securityResult.Result)
                        {
                            continue;
                        }

                        security = this.Db.Securities.Single(s => s.ISIN == portfolioRow.Instrument.ISIN);
                    }

                    // Check if such portfolioRow already exists in the usersPortfolio for this date
                    if (dbPortfolio.SecuritiesPerIssuerCollection.Any(sc => sc.Security.ISIN == security.ISIN))
                    {
                        continue;
                    }

                    // Check if such currency exists and if not create it
                    var currency = this.Db.Currencies.SingleOrDefault(c => c.Code == portfolioRow.Instrument.Currency);
                    if (currency == null)
                    {
                        var currencyResult = this.dataService.CreateCurrency(portfolioRow.Instrument.Currency);
                        if (!currencyResult.Result)
                        {
                            continue;
                        }

                        currency = this.Db.Currencies.Single(c => c.Code == portfolioRow.Instrument.Currency);
                    }

                    // Parse all decimal figures in order to create the SecuritiesPerClient
                    var ifQuantityParsed = decimal.TryParse(portfolioRow.AccountData.Quantity.Replace(" ", ""), out var quantity);
                    var ifAveragePriceBuyParsed = decimal.TryParse(portfolioRow.AccountData.OpenPrice.Replace(" ", ""), out var averagePriceBuy);
                    var ifMarketPriceParsed = decimal.TryParse(portfolioRow.AccountData.MarketPrice.Replace(" ", ""), out var marketPrice);
                    var ifTotalMarketPriceParsed = decimal.TryParse(portfolioRow.AccountData.MarketValue.Replace(" ", ""), out var totalMarketPrice);
                    var ifProfitParsed = decimal.TryParse(portfolioRow.AccountData.Result.Replace(" ", ""), out var profit);
                    var ifProfitInBGNParsed = decimal.TryParse(portfolioRow.AccountData.ResultBGN.Replace(" ", ""), out var profitInBGN);
                    var ifProfitPercentParsed = decimal.TryParse(portfolioRow.Other.YieldPercent.Replace(" ", ""), out var profitPercent);
                    var ifPortfolioShareParsed = decimal.TryParse(portfolioRow.Other.RelativePart.Replace(" ", ""), out var portfolioShare);
                    if (!ifQuantityParsed || !ifAveragePriceBuyParsed ||
                        !ifMarketPriceParsed || !ifTotalMarketPriceParsed ||
                        !ifProfitParsed || !ifProfitInBGNParsed ||
                        !ifProfitPercentParsed || !ifPortfolioShareParsed)
                    {
                        continue;
                    }

                    // Create the SecuritiesPerClient and add it to the DailySecuritiesPerClient
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
                if (!DataValidator.IsValid(dbPortfolio) || !dbPortfolio.SecuritiesPerIssuerCollection.Any())
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
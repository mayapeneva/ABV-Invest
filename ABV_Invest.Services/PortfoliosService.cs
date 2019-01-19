namespace ABV_Invest.Services
{
    using Base;
    using BindingModels.Uploads.Portfolios;
    using Common;
    using Contracts;
    using Data;
    using Models;

    using Mapper = AutoMapper.Mapper;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public class PortfoliosService : BaseService, IPortfoliosService
    {
        private const string initialPass = "789-Asd";
        private const string initialPIN = "00001";
        private const string initialEmail = "client@abv.bg";

        private const string Quantity = "Наличност";
        private const string AveragePrice = "Средна цена";
        private const string MarketPrice = "Пазарна цена";
        private const string MarketValue = "Пазарна стойност";
        private const string Profit = "Доходност";

        private const string ProfitInBgn = "Доходност в лева";
        private const string ProfitInPersentage = "Доходност в %";

        private const string PortfolioShare = "Тегло в портфейла";

        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IBalancesService balancesService;
        private readonly IDataService dataService;

        public PortfoliosService(AbvDbContext db, UserManager<AbvInvestUser> userManager, IBalancesService balancesService, IDataService dataService) : base(db)
        {
            this.userManager = userManager;
            this.balancesService = balancesService;
            this.dataService = dataService;
        }

        public T[] GetUserDailyPortfolio<T>(ClaimsPrincipal user, DateTime date)
        {
            var dbUser = this.userManager.GetUserAsync(user).GetAwaiter().GetResult();
            return dbUser?.Portfolio
                .SingleOrDefault(p => p.Date == date)?
                .SecuritiesPerIssuerCollection
                .Select(Mapper.Map<T>)
                .ToArray();
        }

        public async Task<StringBuilder> SeedPortfolios(IEnumerable<PortfolioRowBindingModel> deserializedPortfolios, DateTime date)
        {
            var changesCounter = 0;
            var mistakes = new StringBuilder();

            // Group the entries by Client and process portfolios for each client
            var portfolios = deserializedPortfolios.GroupBy(p => p.Client.CDNNumber);
            foreach (var portfolio in portfolios)
            {
                // Check if User exists
                var user = this.Db.AbvInvestUsers.SingleOrDefault(u => u.UserName == portfolio.Key);
                if (user == null)
                {
                    mistakes.AppendLine(string.Format(Messages.UserDoesNotExist, portfolio.Key));
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
                            mistakes.AppendLine(string.Format(Messages.SecurityCannotBeCreated, portfolio.Key, securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode, securityInfo.Currency));
                            continue;
                        }

                        security = this.Db.Securities.Single(s => s.ISIN == portfolioRow.Instrument.ISIN);
                    }

                    // Check if such portfolioRow already exists in the usersPortfolio for this date
                    if (dbPortfolio.SecuritiesPerIssuerCollection.Any(sc => sc.Security.ISIN == security.ISIN))
                    {
                        mistakes.AppendLine(string.Format(Messages.SecurityExistsInThisPortfolio, portfolio.Key, date, portfolioRow.Instrument.Issuer, portfolioRow.Instrument.ISIN, portfolioRow.Instrument.NewCode, portfolioRow.Instrument.Currency));
                        continue;
                    }

                    // Check if such currency exists and if not create it
                    var currency = this.Db.Currencies.SingleOrDefault(c => c.Code == portfolioRow.Instrument.Currency);
                    if (currency == null)
                    {
                        var currencyResult = this.dataService.CreateCurrency(portfolioRow.Instrument.Currency);
                        if (!currencyResult.Result)
                        {
                            mistakes.AppendLine(string.Format(Messages.CurrencyCannotBeCreated, portfolioRow.Instrument.Currency, portfolio.Key, portfolioRow.Instrument.Issuer, portfolioRow.Instrument.ISIN, portfolioRow.Instrument.NewCode));
                            continue;
                        }

                        currency = this.Db.Currencies.Single(c => c.Code == portfolioRow.Instrument.Currency);
                    }

                    // Parse all decimal figures in order to create the SecuritiesPerClient
                    var ifQuantityParsed = decimal.TryParse(portfolioRow.AccountData.Quantity.Replace(" ", ""), out var quantity);
                    if (!ifQuantityParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.SecurityCannotBeRegistered, portfolio.Key, Quantity, portfolioRow.AccountData.Quantity));
                        continue;
                    }

                    var ifAveragePriceBuyParsed = decimal.TryParse(portfolioRow.AccountData.OpenPrice.Replace(" ", ""), out var averagePriceBuy);
                    if (!ifAveragePriceBuyParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.SecurityCannotBeRegistered, portfolio.Key, AveragePrice, portfolioRow.AccountData.OpenPrice));
                        continue;
                    }

                    var ifMarketPriceParsed = decimal.TryParse(portfolioRow.AccountData.MarketPrice.Replace(" ", ""), out var marketPrice);
                    if (!ifMarketPriceParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.SecurityCannotBeRegistered, portfolio.Key, MarketPrice, portfolioRow.AccountData.MarketPrice));
                        continue;
                    }

                    var ifTotalMarketPriceParsed = decimal.TryParse(portfolioRow.AccountData.MarketValue.Replace(" ", ""), out var totalMarketPrice);
                    if (!ifTotalMarketPriceParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.SecurityCannotBeRegistered, portfolio.Key, MarketValue, portfolioRow.AccountData.MarketValue));
                        continue;
                    }

                    var ifProfitParsed = decimal.TryParse(portfolioRow.AccountData.Result.Replace(" ", ""), out var profit);
                    if (!ifProfitParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.SecurityCannotBeRegistered, portfolio.Key, Profit, portfolioRow.AccountData.Result));
                        continue;
                    }

                    var ifProfitInBGNParsed = decimal.TryParse(portfolioRow.AccountData.ResultBGN.Replace(" ", ""), out var profitInBGN);
                    if (!ifProfitInBGNParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.SecurityCannotBeRegistered, portfolio.Key, profitInBGN, portfolioRow.AccountData.ResultBGN));
                        continue;
                    }

                    var ifProfitPercentParsed = decimal.TryParse(portfolioRow.Other.YieldPercent.Replace(" ", ""), out var profitPercent);
                    if (!ifProfitPercentParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.SecurityCannotBeRegistered, portfolio.Key, ProfitInPersentage, portfolioRow.Other.YieldPercent));
                        continue;
                    }

                    var ifPortfolioShareParsed = decimal.TryParse(portfolioRow.Other.RelativePart.Replace(" ", ""), out var portfolioShare);
                    if (!ifPortfolioShareParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.SecurityCannotBeRegistered, portfolio.Key, PortfolioShare, portfolioRow.Other.RelativePart));
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
                        mistakes.AppendLine(string.Format(Messages.SecurityCannotBeCreated, portfolio.Key, portfolioRow.Instrument.Issuer, portfolioRow.Instrument.ISIN, portfolioRow.Instrument.NewCode, portfolioRow.Instrument.Currency));
                        continue;
                    }
                    dbPortfolio.SecuritiesPerIssuerCollection.Add(securitiesPerClient);
                }

                // Validate portfolio and add it to user's Portfolios
                if (!DataValidator.IsValid(dbPortfolio) || !dbPortfolio.SecuritiesPerIssuerCollection.Any())
                {
                    mistakes.AppendLine(string.Format(Messages.PortfolioCannotBeCreated, portfolio.Key, date));
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

            var finalResult = new StringBuilder();
            finalResult.AppendLine(changesCounter == 0 ? Messages.CouldNotUploadInformation : Messages.UploadingSuccessfull);
            finalResult.Append(mistakes);

            return finalResult;
        }
    }
}
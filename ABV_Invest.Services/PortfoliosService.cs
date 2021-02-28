namespace ABV_Invest.Services
{
    using ABV_Invest.Common.BindingModels.Uploads.Portfolios;
    using AutoMapper;
    using Base;
    using Common;
    using Contracts;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public class PortfoliosService : BaseService, IPortfoliosService
    {
        private const string Quantity = "Наличност";
        private const string AveragePrice = "Средна цена";
        private const string MarketPrice = "Пазарна цена";
        private const string MarketValue = "Пазарна стойност";
        private const string Profit = "Доходност";
        private const string SvSeCulture = "sv-SE";

        private const string ProfitInPersentage = "Доходност в %";

        private const string PortfolioShare = "Тегло в портфейла";

        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IBalancesService balancesService;
        private readonly IDataService dataService;

        public PortfoliosService(
            AbvDbContext db,
            UserManager<AbvInvestUser> userManager,
            IBalancesService balancesService,
            IDataService dataService)
            : base(db)
        {
            this.userManager = userManager;
            this.balancesService = balancesService;
            this.dataService = dataService;
        }

        public async Task<T[]> GetUserDailyPortfolio<T>(ClaimsPrincipal user, DateTime date)
        {
            var dbUser = await this.userManager.GetUserAsync(user);
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
                if (user is null)
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
                    var portfolioResult = await this.CreatePortfolioRowForUser(date, user, portfolioRow, portfolio.Key, dbPortfolio);
                    if (string.IsNullOrWhiteSpace(portfolioResult))
                    {
                        mistakes.AppendLine(portfolioResult);
                    }
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

        private async Task<string> CreatePortfolioRowForUser(DateTime date, AbvInvestUser user, PortfolioRowBindingModel portfolioRow,
            string portfolioKey, DailySecuritiesPerClient dbPortfolio)
        {
            // Fill in user's FullName if empty
            if (String.IsNullOrWhiteSpace(user.FullName))
            {
                user.FullName = portfolioRow.Client.Name;
            }

            var securityInfo = portfolioRow.Instrument;
            // Get or create security
            var security = await this.GetOrCreateSecurity(securityInfo);
            if (security is null)
            {
                return string.Format(Messages.SecurityCannotBeCreated, portfolioKey, securityInfo.Issuer,
                    securityInfo.ISIN, securityInfo.NewCode, securityInfo.Currency);
            }

            // Check if such portfolioRow already exists in the usersPortfolio for this date
            if (dbPortfolio.SecuritiesPerIssuerCollection.Any(sc => sc.Security.ISIN == security.ISIN))
            {
                return string.Format(Messages.SecurityExistsInThisPortfolio, portfolioKey, date,
                    securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode,
                    securityInfo.Currency);
            }

            // Get or create currency
            var currency = await this.GetOrCreateCurrency(securityInfo);
            if (currency is null)
            {
                return string.Format(Messages.CurrencyCannotBeCreated, securityInfo.Currency,
                        portfolioKey, securityInfo.Issuer, securityInfo.ISIN,
                        securityInfo.NewCode);
            }

            var accountData = portfolioRow.AccountData;
            // Parse data and create SecuritiesPerClient
            var securityResult = this.ParseDataAndCreateSecuritiesPerClient(portfolioRow, accountData,
                security, currency, out var securitiesPerClient);
            if (securitiesPerClient is null)
            {
                return string.Format(Messages.SecurityCannotBeRegistered, portfolioKey, securityResult[0], securityResult[1]);
            }

            // Validate SecuritiesPerClient and add them to the portfolio
            if (!DataValidator.IsValid(securitiesPerClient))
            {
                return string.Format(Messages.SecurityCannotBeCreated, portfolioKey,
                    securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode,
                    securityInfo.Currency);
            }

            dbPortfolio.SecuritiesPerIssuerCollection.Add(securitiesPerClient);
            return null;
        }

        private async Task<Security> GetOrCreateSecurity(Instrument securityInfo)
        {
            var security = this.Db.Securities.SingleOrDefault(s => s.ISIN == securityInfo.ISIN);
            if (security is null)
            {
                var securityResult = await this.dataService.CreateSecurity(securityInfo.Issuer, securityInfo.ISIN,
                    securityInfo.NewCode, securityInfo.Currency);
                if (!securityResult)
                {
                    return null;
                }

                security = this.Db.Securities.Single(s => s.ISIN == securityInfo.ISIN);
            }

            return security;
        }

        private async Task<Currency> GetOrCreateCurrency(Instrument securityInfo)
        {
            var currency = this.Db.Currencies.SingleOrDefault(c => c.Code == securityInfo.Currency);
            if (currency is null)
            {
                var currencyResult = await this.dataService.CreateCurrency(securityInfo.Currency);
                if (!currencyResult)
                {
                    return null;
                }

                currency = this.Db.Currencies.Single(c => c.Code == securityInfo.Currency);
            }

            return currency;
        }

        private string[] ParseDataAndCreateSecuritiesPerClient(PortfolioRowBindingModel portfolioRow, AccountData accountData,
            Security security, Currency currency, out SecuritiesPerClient securitiesPerClient)
        {
            var ifQuantityParsed = decimal.TryParse(accountData.Quantity.Replace(" ", string.Empty), out var quantity);
            if (!ifQuantityParsed)
            {
                securitiesPerClient = null;
                return new[] { Quantity, accountData.Quantity };
            }

            var ifAveragePriceBuyParsed = decimal.TryParse(accountData.OpenPrice.Replace(" ", string.Empty), out var averagePriceBuy);
            if (!ifAveragePriceBuyParsed)
            {
                securitiesPerClient = null;
                return new[] { AveragePrice, accountData.OpenPrice };
            }

            var ifMarketPriceParsed = decimal.TryParse(accountData.MarketPrice.Replace(" ", string.Empty), out var marketPrice);
            if (!ifMarketPriceParsed)
            {
                securitiesPerClient = null;
                return new[] { MarketPrice, accountData.MarketPrice };
            }

            var ifTotalMarketPriceParsed = decimal.TryParse(accountData.MarketValue.Replace(" ", string.Empty), out var totalMarketPrice);
            if (!ifTotalMarketPriceParsed)
            {
                securitiesPerClient = null;
                return new[] { MarketValue, accountData.MarketValue };
            }

            var ifProfitParsed = decimal.TryParse(accountData.Result.Replace(" ", string.Empty), out var profit);
            if (!ifProfitParsed)
            {
                securitiesPerClient = null;
                return new[] { Profit, accountData.Result };
            }

            var ifProfitInBGNParsed = decimal.TryParse(accountData.ResultBGN.Replace(" ", string.Empty), out var profitInBGN);
            if (!ifProfitInBGNParsed)
            {
                securitiesPerClient = null;
                return new[] { profitInBGN.ToString("N2", CultureInfo.CreateSpecificCulture(SvSeCulture)), accountData.ResultBGN };
            }

            var ifProfitPercentParsed = decimal.TryParse(portfolioRow.Other.YieldPercent.Replace(" ", string.Empty), out var profitPercent);
            if (!ifProfitPercentParsed)
            {
                securitiesPerClient = null;
                return new[] { ProfitInPersentage, portfolioRow.Other.YieldPercent };
            }

            var ifPortfolioShareParsed = decimal.TryParse(portfolioRow.Other.RelativePart.Replace(" ", string.Empty), out var portfolioShare);
            if (!ifPortfolioShareParsed)
            {
                securitiesPerClient = null;
                return new[] { PortfolioShare, portfolioRow.Other.RelativePart };
            }

            // Create the SecuritiesPerClient and add it to the DailySecuritiesPerClient
            securitiesPerClient = new SecuritiesPerClient
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

            return null;
        }
    }
}
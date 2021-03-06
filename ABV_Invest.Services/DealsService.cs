﻿namespace ABV_Invest.Services
{
    using ABV_Invest.Common.BindingModels.Uploads.Deals;
    using AutoMapper;
    using Base;
    using Common;
    using Contracts;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Models;
    using Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public class DealsService : BaseService, IDealsService
    {
        private const string TypeOfDeal = "Тип на сделката";
        private const string Quantity = "Количество";
        private const string Price = "Цена";
        private const string Coupon = "Купон";
        private const string CurrencyValue = "Стойност в дадената валута";
        private const string BgnValue = "Стойност в лева";
        private const string Fee = "Комисионна";
        private const string Settlement = "Сетълмент";

        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IDataService dataService;

        public DealsService(
            AbvDbContext db,
            UserManager<AbvInvestUser> userManager,
            IDataService dataService)
            : base(db)
        {
            this.userManager = userManager;
            this.dataService = dataService;
        }

        public async Task<T[]> GetUserDailyDeals<T>(ClaimsPrincipal user, DateTime date)
        {
            var dbUser = await this.userManager.GetUserAsync(user);
            return dbUser?.Deals
                .SingleOrDefault(p => p.Date == date)
                ?.Deals
                .Select(d => Mapper.Map<T>(d))
                .ToArray();
        }

        public async Task<StringBuilder> SeedDeals(DealRowBindingModel[] deserializedDeals, DateTime date)
        {
            var changesCounter = 0;
            var mistakes = new StringBuilder();

            // Group the entries by Client and process deals for each client
            var deals = deserializedDeals.GroupBy(p => p.Client.CDNNumber);
            foreach (var deal in deals)
            {
                // Check if User exists
                var user = this.Db.AbvInvestUsers.SingleOrDefault(u => u.UserName == deal.Key);
                if (user is null)
                {
                    mistakes.AppendLine(string.Format(Messages.UserDoesNotExist, deal.Key));
                    continue;
                }

                // Check if there is a DailyDealsEntity created for this User and date already
                if (this.Db.DailyDeals.Any(dd => dd.AbvInvestUserId == user.Id && dd.Date == date))
                {
                    mistakes.AppendLine(string.Format(Messages.DailyDealsAlredyExist, deal.Key, date));
                    continue;
                }

                // Create new DailyDealsEntity
                var dbDailyDeals = new DailyDeals
                {
                    Date = date,
                    Deals = new HashSet<Deal>()
                };

                // Create all Deals for this User
                foreach (var dealRow in deal)
                {
                    var dealRowResult = await this.CreateDealRowForUser(dealRow, deal.Key, dbDailyDeals);
                    if (string.IsNullOrWhiteSpace(dealRowResult))
                    {
                        mistakes.AppendLine(dealRowResult);
                    }
                }

                // Validate dailyDeals
                if (!DataValidator.IsValid(dbDailyDeals) || !dbDailyDeals.Deals.Any())
                {
                    mistakes.AppendLine(string.Format(Messages.DailyDealsCannotBeCreated, deal.Key, date));
                    continue;
                }

                //Add dailyDeals to user's Deals
                user.Deals.Add(dbDailyDeals);
                changesCounter += await this.Db.SaveChangesAsync();
            }

            var finalResult = new StringBuilder();
            finalResult.AppendLine(changesCounter == 0 ? Messages.CouldNotUploadInformation : Messages.UploadingSuccessfull);
            finalResult.Append(mistakes);

            return finalResult;
        }

        private async Task<string> CreateDealRowForUser(DealRowBindingModel dealRow, string dealKey,
            DailyDeals dbDailyDeals)
        {
            var securityInfo = dealRow.Instrument;
            // Get or create security
            var security = await this.GetOrCreateSecurity(securityInfo);
            if (security is null)
            {
                return string.Format(Messages.SecurityCannotBeCreated, dealKey, securityInfo.Issuer,
                    securityInfo.ISIN, securityInfo.NewCode, securityInfo.Currency);
            }

            // Get or create currency
            var currency = await this.GetOrCreateCurrency(securityInfo);
            if (currency is null)
            {
                return string.Format(Messages.CurrencyCannotBeCreated, securityInfo.Currency, dealKey,
                    securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode);
            }

            // Check if such market exists
            var dealData = dealRow.DealData;
            var market = this.Db.Markets.SingleOrDefault(m => m.MIC == dealData.StockExchangeMIC);
            if (market is null)
            {
                return string.Format(Messages.MarketDoesNotExist, dealData.Operation,
                    securityInfo.ISIN, dealKey, dealData.StockExchangeMIC);
            }

            // Parse data and create deal
            var dealResult = this.ParseDataAndCreateDeal(dealData, security, currency, market, out var dbDeal);
            if (dbDeal is null)
            {
                return string.Format(Messages.DealCannotBeRegistered, dealData.Operation,
                    securityInfo.ISIN, dealKey, dealResult[0], dealResult[1]);
            };

            // Validate the Deal and add it to the dailyDeals
            if (!DataValidator.IsValid(dbDeal))
            {
                return string.Format(Messages.DealRowCannotBeCreated,
                    dealData.Operation,
                    securityInfo.ISIN,
                    dealKey,
                    dealData.ShareCount,
                    dealData.SinglePrice);
            }

            dbDailyDeals.Deals.Add(dbDeal);

            return null;
        }

        private async Task<Security> GetOrCreateSecurity(Instrument securityInfo)
        {
            var security =
                this.Db.Securities.SingleOrDefault(s => s.ISIN == securityInfo.ISIN);
            if (security is null)
            {
                var securityResult = await this.dataService.CreateSecurity(
                    securityInfo.Issuer,
                    securityInfo.ISIN,
                    securityInfo.NewCode,
                    securityInfo.Currency);
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

        private string[] ParseDataAndCreateDeal(DealData dealData, Security security, Currency currency, Market market, out Deal dbDeal)
        {
            var operation = dealData.Operation;
            if (operation != Constants.Buy && operation != Constants.Sell)
            {
                dbDeal = null;
                return new[] { TypeOfDeal, operation };
            }
            var dealType = operation == Constants.Buy ? DealType.Купува : DealType.Продава;

            var ifQuantityParsed = decimal.TryParse(dealData.ShareCount.Replace(" ", string.Empty), out var quantity);
            if (!ifQuantityParsed)
            {
                dbDeal = null;
                return new[] { Quantity, dealData.ShareCount };
            }

            var ifPriceParsed = decimal.TryParse(dealData.SinglePrice.Replace(" ", string.Empty), out var price);
            if (!ifPriceParsed)
            {
                dbDeal = null;
                return new[] { Price, dealData.SinglePrice };
            }

            var ifCouponParsed = decimal.TryParse(dealData.Coupon.Replace(" ", string.Empty), out var coupon);
            if (!ifCouponParsed)
            {
                dbDeal = null;
                return new[] { Coupon, dealData.Coupon };
            }

            var ifTotalPriceParsed = decimal.TryParse(dealData.DealAmountInShareCurrency.Replace(" ", string.Empty), out var totalPrice);
            if (!ifTotalPriceParsed)
            {
                dbDeal = null;
                return new[] { CurrencyValue, dealData.DealAmountInShareCurrency };
            }

            var ifTotalPriceInBGNParsed = decimal.TryParse(dealData.DealAmountInPaymentCurrency.Replace(" ", string.Empty), out var totalPriceInBGN);
            if (!ifTotalPriceInBGNParsed)
            {
                dbDeal = null;
                return new[] { BgnValue, dealData.DealAmountInPaymentCurrency };
            }

            var ifFeeParsed = decimal.TryParse(dealData.CommissionInPaymentCurrency.Replace(" ", string.Empty), out var fee);
            if (!ifFeeParsed)
            {
                dbDeal = null;
                return new[] { Fee, dealData.CommissionInPaymentCurrency };
            }

            var ifSettlementParsed = DateTime.TryParse(dealData.DeliveryDate, out DateTime settlement);
            if (!ifSettlementParsed)
            {
                dbDeal = null;
                return new[] { Settlement, dealData.DeliveryDate };
            }

            dbDeal = new Deal
            {
                DealType = dealType,
                Security = security,
                Quantity = quantity,
                Price = price,
                Coupon = coupon,
                TotalPrice = totalPrice,
                TotalPriceInBGN = totalPriceInBGN,
                Fee = fee,
                Currency = currency,
                Settlement = settlement,
                Market = market
            };

            return null;
        }
    }
}
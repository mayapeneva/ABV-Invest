namespace ABV_Invest.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AutoMapper;
    using Base;
    using BindingModels.Uploads.Deals;
    using Common;
    using Contracts;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Models;
    using Models.Enums;

    public class DealsService : BaseService, IDealsService
    {
        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IDataService dataService;

        public DealsService(AbvDbContext db, UserManager<AbvInvestUser> userManager, IDataService dataService) : base(db)
        {
            this.userManager = userManager;
            this.dataService = dataService;
        }

        public T[] GetUserDailyDeals<T>(ClaimsPrincipal user, string chosenDate)
        {
            var date = DateTime.Parse(chosenDate);
            var dbUser = this.userManager.GetUserAsync(user).GetAwaiter().GetResult();
            return dbUser?.Deals
                .SingleOrDefault(p => p.Date == date)
                ?.Deals
                .Select(Mapper.Map<T>)
                .ToArray();
        }

        public async Task<bool> SeedDeals(DealRowBindingModel[] deserializedDeals, DateTime date)
        {
            var changesCounter = 0;

            // Group the entries by Client and process deals for each client
            var deals = deserializedDeals.GroupBy(p => p.Client.CDNNumber);
            foreach (var deal in deals)
            {
                // Check if User exists
                var user = this.Db.AbvInvestUsers.SingleOrDefault(u => u.UserName == deal.Key);
                if (user == null)
                {
                    continue;
                }

                // Check if there is a DailyDealsEntity created for this User and date already
                if (this.Db.DailyDeals.Any(dd => dd.AbvInvestUserId == user.Id && dd.Date == date))
                {
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
                    // Check if such security exists and if not create it
                    var security =
                        this.Db.Securities.SingleOrDefault(s => s.ISIN == dealRow.Instrument.ISIN);
                    if (security == null)
                    {
                        var securityInfo = dealRow.Instrument;
                        var securityResult = this.dataService.CreateSecurity(securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode,
                            securityInfo.Currency);
                        if (!securityResult.Result)
                        {
                            continue;
                        }

                        security = this.Db.Securities.Single(s => s.ISIN == dealRow.Instrument.ISIN);
                    }

                    // Check if such currency exists and if not create it
                    var currency = this.Db.Currencies.SingleOrDefault(c => c.Code == dealRow.Instrument.Currency);
                    if (currency == null)
                    {
                        var currencyResult = this.dataService.CreateCurrency(dealRow.Instrument.Currency);
                        if (!currencyResult.Result)
                        {
                            continue;
                        }

                        currency = this.Db.Currencies.Single(c => c.Code == dealRow.Instrument.Currency);
                    }

                    // Check if such market exists
                    var market = this.Db.Markets.SingleOrDefault(m => m.MIC == dealRow.DealData.StockExchangeMIC);
                    if (market == null)
                    {
                        continue;
                    }

                    // Parse all dates, enums and decimal figures in order to create the Deal
                    var ifDealTypeParsed = dealRow.DealData.Operation == "BUY" || dealRow.DealData.Operation == "SELL";
                    var dealType = dealRow.DealData.Operation == "BUY" ? DealType.Купува : DealType.Продава;
                    var ifQuantityParsed = decimal.TryParse(dealRow.DealData.ShareCount.Replace(" ", ""), out var quantity);
                    var ifPriceParsed = decimal.TryParse(dealRow.DealData.SinglePrice.Replace(" ", ""), out var price);
                    var ifCouponParsed = decimal.TryParse(dealRow.DealData.Coupon.Replace(" ", ""), out var coupon);
                    var ifTotalPriceParsed = decimal.TryParse(dealRow.DealData.DealAmountInShareCurrency.Replace(" ", ""), out var totalPrice);
                    var ifTotalPriceInBGNParsed = decimal.TryParse(dealRow.DealData.DealAmountInPaymentCurrency.Replace(" ", ""), out var totalPriceInBGN);
                    var ifFeeParsed = decimal.TryParse(dealRow.DealData.CommissionInPaymentCurrency.Replace(" ", ""), out var fee);
                    var ifSettlementParsed = DateTime.TryParse(dealRow.DealData.DeliveryDate, out DateTime settlement);

                    if (!ifDealTypeParsed || !ifQuantityParsed ||
                        !ifPriceParsed || !ifCouponParsed ||
                        !ifTotalPriceParsed || !ifTotalPriceInBGNParsed ||
                        !ifFeeParsed || !ifSettlementParsed)
                    {
                        continue;
                    }

                    // Create the Deal and add it to the DailyDeals
                    var dbDeal = new Deal
                    {
                        DealType = (DealType)dealType,
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

                    // Validate Deal and add it to the dailyDeals
                    if (!DataValidator.IsValid(dbDeal))
                    {
                        continue;
                    }
                    dbDailyDeals.Deals.Add(dbDeal);
                }

                // Validate dailyDeals and add them to user's Deals
                if (!DataValidator.IsValid(dbDailyDeals) || !dbDailyDeals.Deals.Any())
                {
                    continue;
                }
                user.Deals.Add(dbDailyDeals);
                changesCounter += await this.Db.SaveChangesAsync();
            }

            if (changesCounter == 0)
            {
                return false;
            }

            return true;
        }
    }
}
namespace ABV_Invest.Services
{
    using AutoMapper;
    using Base;
    using BindingModels.Uploads.Deals;
    using Common;
    using Contracts;
    using Data;
    using Models;
    using Models.Enums;

    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

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
                if (user == null)
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
                            mistakes.AppendLine(string.Format(Messages.SecurityCannotBeCreated, deal.Key, securityInfo.Issuer, securityInfo.ISIN, securityInfo.NewCode, securityInfo.Currency));
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
                            mistakes.AppendLine(string.Format(Messages.CurrencyCannotBeCreated, dealRow.Instrument.Currency, deal.Key, dealRow.Instrument.Issuer, dealRow.Instrument.ISIN, dealRow.Instrument.NewCode));
                            continue;
                        }

                        currency = this.Db.Currencies.Single(c => c.Code == dealRow.Instrument.Currency);
                    }

                    // Check if such market exists
                    var market = this.Db.Markets.SingleOrDefault(m => m.MIC == dealRow.DealData.StockExchangeMIC);
                    if (market == null)
                    {
                        mistakes.AppendLine(string.Format(Messages.MarketDoesNotExist, dealRow.DealData.Operation, dealRow.Instrument.ISIN, deal.Key, dealRow.DealData.StockExchangeMIC));
                        continue;
                    }

                    // Parse all dates, enums and decimal figures in order to create the Deal
                    var ifDealTypeParsed = dealRow.DealData.Operation == "BUY" || dealRow.DealData.Operation == "SELL";
                    if (!ifDealTypeParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.DealCannotBeRegistered, dealRow.DealData.Operation, dealRow.Instrument.ISIN, deal.Key, "Тип на сделката", dealRow.DealData.Operation));
                        continue;
                    }
                    var dealType = dealRow.DealData.Operation == "BUY" ? DealType.Купува : DealType.Продава;

                    var ifQuantityParsed = decimal.TryParse(dealRow.DealData.ShareCount.Replace(" ", ""), out var quantity);
                    if (!ifQuantityParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.DealCannotBeRegistered, dealRow.DealData.Operation, dealRow.Instrument.ISIN, deal.Key, "Количество", dealRow.DealData.ShareCount));
                        continue;
                    }

                    var ifPriceParsed = decimal.TryParse(dealRow.DealData.SinglePrice.Replace(" ", ""), out var price);
                    if (!ifPriceParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.DealCannotBeRegistered, dealRow.DealData.Operation, dealRow.Instrument.ISIN, deal.Key, "Цена", dealRow.DealData.SinglePrice));
                        continue;
                    }

                    var ifCouponParsed = decimal.TryParse(dealRow.DealData.Coupon.Replace(" ", ""), out var coupon);
                    if (!ifCouponParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.DealCannotBeRegistered, dealRow.DealData.Operation, dealRow.Instrument.ISIN, deal.Key, "Купон", dealRow.DealData.Coupon));
                        continue;
                    }

                    var ifTotalPriceParsed = decimal.TryParse(dealRow.DealData.DealAmountInShareCurrency.Replace(" ", ""), out var totalPrice);
                    if (!ifTotalPriceParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.DealCannotBeRegistered, dealRow.DealData.Operation, dealRow.Instrument.ISIN, deal.Key, "Стойност в дадената валута", dealRow.DealData.DealAmountInShareCurrency));
                        continue;
                    }

                    var ifTotalPriceInBGNParsed = decimal.TryParse(dealRow.DealData.DealAmountInPaymentCurrency.Replace(" ", ""), out var totalPriceInBGN);
                    if (!ifTotalPriceInBGNParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.DealCannotBeRegistered, dealRow.DealData.Operation, dealRow.Instrument.ISIN, deal.Key, "Стойност в лева", dealRow.DealData.DealAmountInPaymentCurrency));
                        continue;
                    }

                    var ifFeeParsed = decimal.TryParse(dealRow.DealData.CommissionInPaymentCurrency.Replace(" ", ""), out var fee);
                    if (!ifFeeParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.DealCannotBeRegistered, dealRow.DealData.Operation, dealRow.Instrument.ISIN, deal.Key, "Комисионна", dealRow.DealData.CommissionInPaymentCurrency));
                        continue;
                    }

                    var ifSettlementParsed = DateTime.TryParse(dealRow.DealData.DeliveryDate, out DateTime settlement);
                    if (!ifSettlementParsed)
                    {
                        mistakes.AppendLine(string.Format(Messages.DealCannotBeRegistered, dealRow.DealData.Operation, dealRow.Instrument.ISIN, deal.Key, "Сетълмент", dealRow.DealData.DeliveryDate));
                        continue;
                    }

                    // Create the Deal
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

                    // Validate the Deal and add it to the dailyDeals
                    if (!DataValidator.IsValid(dbDeal))
                    {
                        mistakes.AppendLine(string.Format(Messages.DealRowCannotBeCreated, dealRow.DealData.Operation, dealRow.Instrument.ISIN, deal.Key, dealRow.DealData.ShareCount, dealRow.DealData.SinglePrice));
                        continue;
                    }
                    dbDailyDeals.Deals.Add(dbDeal);
                }

                // Validate dailyDeals and add them to user's Deals
                if (!DataValidator.IsValid(dbDailyDeals) || !dbDailyDeals.Deals.Any())
                {
                    mistakes.AppendLine(string.Format(Messages.DailyDealsCannotBeCreated, deal.Key, date));
                    continue;
                }
                user.Deals.Add(dbDailyDeals);
                changesCounter += await this.Db.SaveChangesAsync();
            }

            var finalResult = new StringBuilder();
            finalResult.AppendLine(changesCounter == 0 ? Messages.CouldNotUploadInformation : Messages.UploadingSuccessfull);
            finalResult.Append(mistakes);

            return finalResult;
        }
    }
}
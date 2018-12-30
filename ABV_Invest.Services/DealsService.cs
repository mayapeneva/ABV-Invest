namespace ABV_Invest.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Base;
    using BindingModels.Uploads.Deals;
    using Common;
    using Contracts;
    using Data;
    using Models;
    using Models.Enums;

    public class DealsService : BaseService, IDealsService
    {
        public DealsService(AbvDbContext db)
            : base(db)
        {
        }

        public T[] GetUserDailyDeals<T>(string userId, string chosenDate)
        {
            var ifParsed = DateTime.TryParse(chosenDate, out DateTime date);
            if (!ifParsed)
            {
                return null;
            }

            var deals = this.Db.DailyDeals.SingleOrDefault(p =>
                p.AbvInvestUserId == userId && p.Date == date);

            var collection = deals?.Deals.Select(Mapper.Map<T>).ToArray();

            return collection;
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
                    // Check if such security exists
                    var security =
                        this.Db.Securities.SingleOrDefault(s => s.ISIN == dealRow.Instrument.ISIN);
                    if (security == null)
                    {
                        continue;
                    }

                    // Create the Deal and add it to the DailyDeals
                    var dealType = dealRow.DealData.Operation == "BUY" ? DealType.Купува : DealType.Продава;
                    var quantity = decimal.Parse(dealRow.DealData.ShareCount.Replace(" ", ""));
                    var price = decimal.Parse(dealRow.DealData.SinglePrice.Replace(" ", ""));
                    var coupon = decimal.Parse(dealRow.DealData.Coupon.Replace(" ", ""));
                    var totalPrice = decimal.Parse(dealRow.DealData.DealAmountInShareCurrency.Replace(" ", ""));
                    var totalPriceInBGN = decimal.Parse(dealRow.DealData.DealAmountInPaymentCurrency.Replace(" ", ""));
                    var fee = decimal.Parse(dealRow.DealData.CommissionInPaymentCurrency.Replace(" ", ""));
                    var currency = this.Db.Currencies.SingleOrDefault(c => c.Code == dealRow.Instrument.Currency);
                    if (currency == null)
                    {
                        continue;
                    }

                    var ifParsed = DateTime.TryParse(dealRow.DealData.DeliveryDate, out DateTime settlement);
                    if (!ifParsed)
                    {
                        continue;
                    }

                    var market = this.Db.Markets.SingleOrDefault(m => m.MIC == dealRow.DealData.StockExchangeMIC);
                    if (market == null)
                    {
                        continue;
                    }

                    var dbDeal = new Deal
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

                    // Validate Deal and add it to the dailyDeals
                    if (!DataValidator.IsValid(dbDeal))
                    {
                        continue;
                    }
                    dbDailyDeals.Deals.Add(dbDeal);
                }

                // Validate dailyDeals and add them to user's Deals
                if (!DataValidator.IsValid(dbDailyDeals))
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
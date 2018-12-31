namespace ABV_Invest.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Moq;
    using Xunit;

    public class BalancesServiceTests
    {
        private readonly AbvDbContext Db;
        private readonly IBalancesService balanacesService;

        public BalancesServiceTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV")
                .Options;
            this.Db = new AbvDbContext(options);

            this.balanacesService = new BalancesService(this.Db);
        }

        [Fact]
        public async Task BalancesServiceShouldCreateBalanceForUser()
        {
            var date = new DateTime(2018, 12, 28);
            var moqUser = new Mock<AbvInvestUser>();
            moqUser.Setup(u => u.Balances).Returns(new HashSet<DailyBalance>());
            moqUser.Setup(u => u.Portfolio).Returns(new HashSet<DailySecuritiesPerClient> { new DailySecuritiesPerClient
            {
                Date = date,
                SecuritiesPerIssuerCollection = new HashSet<SecuritiesPerClient> { new SecuritiesPerClient
            {
                Quantity = 100,
                AveragePriceBuy = 10000,
                MarketPrice = 200,
                TotalMarketPrice = 20000,
                Profit = 10000,
                ProfitInBGN = 10000,
                ProfitPercentàge = 100,
                PortfolioShare = 10
            }
                }}});
            var expected = 1;

            await this.balanacesService.CreateBalanceForUser(moqUser.Object, date);
            var actual = moqUser.Object.Balances.Count;

            Assert.Equal(expected, actual);
        }
    }
}
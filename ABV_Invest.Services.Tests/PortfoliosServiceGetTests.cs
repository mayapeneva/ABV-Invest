namespace ABV_Invest.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Data;
    using DTOs;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Moq;
    using Xunit;

    public class PortfoliosServiceGetTests
    {
        private const string Date = "28/12/2018";

        private readonly AbvDbContext db;
        private readonly IPortfoliosService portfoliosService;
        private readonly Mock<AbvInvestUser> moqUser;

        public PortfoliosServiceGetTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV")
                .Options;
            this.db = new AbvDbContext(options);

            AutoMapperConfig.RegisterMappings(
                typeof(PortfolioDto).Assembly);

            var balancesService = new BalancesService(this.db);
            var dataService = new DataService(this.db);
            this.portfoliosService = new PortfoliosService(this.db, balancesService, dataService);

            var date = new DateTime(2018, 12, 28);
            this.moqUser = new Mock<AbvInvestUser>();
            this.moqUser.Setup(u => u.Portfolio).Returns(new HashSet<DailySecuritiesPerClient> { new DailySecuritiesPerClient
            {
                Date = date,
                SecuritiesPerIssuerCollection = new HashSet<SecuritiesPerClient> { new SecuritiesPerClient
                    {
                        Quantity = 100,
                        AveragePriceBuy = 100,
                        MarketPrice = 200,
                        TotalMarketPrice = 20000,
                        Profit = 10000,
                        ProfitInBGN = 10000,
                        ProfitPercentаge = 100,
                        PortfolioShare = 10
                    }
                }
            }});
        }

        [Fact]
        public void GetUserDailyPortfolio_ShouldReturnDailyPortfolio()
        {
            // Act
            var result = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.moqUser.Object, Date);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetUserDailyPortfolio_ShouldReturnDailyPortfolioWithCorrectTotalMarketPrice()
        {
            // Arange
            var expectedTotalMarketPrice =
                this.moqUser.Object.Portfolio.Select(p => p.SecuritiesPerIssuerCollection.Sum(s => s.TotalMarketPrice));

            // Act
            var actualTotalMarketPrice = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.moqUser.Object, Date).Select(p => p.TotalMarketPrice);

            // Assert
            Assert.Equal(expectedTotalMarketPrice, actualTotalMarketPrice);
        }

        [Fact]
        public void GetUserDailyPortfolio_ShouldReturnNullIfThereIsNoPortfolioForThisDate()
        {
            // Arange
            var date = "27/12/2018";

            // Act
            var result = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.moqUser.Object, date);

            // Assert
            Assert.Null(result);
        }
    }
}
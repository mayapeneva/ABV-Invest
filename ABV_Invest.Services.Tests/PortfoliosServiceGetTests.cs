namespace ABV_Invest.Services.Tests
{
    using ABV_Invest.Common.DTOs;
    using ABV_Invest.Common.Mapping;
    using Contracts;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Xunit;

    public class PortfoliosServiceGetTests
    {
        private readonly AbvDbContext db;
        private readonly IPortfoliosService portfoliosService;
        private readonly Mock<AbvInvestUser> moqUser;
        private readonly ClaimsPrincipal principal;

        private DateTime Date = new DateTime(2020, 12, 15);

        public PortfoliosServiceGetTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV")
                .Options;
            db = new AbvDbContext(options);

            AutoMapperConfig.RegisterMappings(
                typeof(PortfolioDto).Assembly);

            moqUser = new Mock<AbvInvestUser>();
            moqUser.Setup(u => u.Portfolio).Returns(new HashSet<DailySecuritiesPerClient> { new DailySecuritiesPerClient
            {
                Date = Date,
                SecuritiesPerIssuerCollection = new HashSet<SecuritiesPerClient> { new SecuritiesPerClient
                    {
                        Quantity = 100,
                        AveragePriceBuy = 100,
                        MarketPrice = 200,
                        TotalMarketPrice = 20000,
                        Profit = 10000,
                        ProfitInBGN = 10000,
                        ProfitPercentage = 100,
                        PortfolioShare = 10
                    }
                }
            }});

            var balancesService = new BalancesService(db);
            var dataService = new DataService(db);
            var mockUserStore = new Mock<IUserStore<AbvInvestUser>>();
            var userManager = new Mock<UserManager<AbvInvestUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            principal = new ClaimsPrincipal();
            userManager.Setup(um => um.GetUserAsync(principal)).Returns(Task.FromResult(moqUser.Object));
            portfoliosService = new PortfoliosService(db, userManager.Object, balancesService, dataService);
        }

        [Fact]
        public void GetUserDailyPortfolio_ShouldReturnDailyPortfolio()
        {
            // Act
            var result = portfoliosService.GetUserDailyPortfolio<PortfolioDto>(principal, Date);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetUserDailyPortfolio_ShouldReturnDailyPortfolioWithCorrectTotalMarketPrice()
        {
            // Arange
            var expectedTotalMarketPrice =
                moqUser.Object.Portfolio.Select(p => p.SecuritiesPerIssuerCollection.Sum(s => s.TotalMarketPrice));

            // Act
            var totalMarketPrice = await portfoliosService.GetUserDailyPortfolio<PortfolioDto>(principal, Date);
            var actualTotalMarketPrice = totalMarketPrice.Select(p => p.TotalMarketPrice);

            // Assert
            Assert.Equal(expectedTotalMarketPrice, actualTotalMarketPrice);
        }

        [Fact]
        public async Task GetUserDailyPortfolio_ShouldReturnNullIfThereIsNoPortfolioForThisDate()
        {
            // Arange
            var date = new DateTime(2020, 12, 27);

            // Act
            var result = await portfoliosService.GetUserDailyPortfolio<PortfolioDto>(principal, date);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserDailyPortfolio_ShouldReturnNullIfThereIsNoSuchUser()
        {
            // Arange
            var user = new ClaimsPrincipal();

            // Act
            var result = await portfoliosService.GetUserDailyPortfolio<PortfolioDto>(user, Date);

            // Assert
            Assert.Null(result);
        }
    }
}
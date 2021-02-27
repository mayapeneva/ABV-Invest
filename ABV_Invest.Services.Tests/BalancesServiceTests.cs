namespace ABV_Invest.Services.Tests
{
    using ABV_Invest.Common.DTOs;
    using ABV_Invest.Common.Mapping;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class BalancesServiceTests
    {
        private readonly AbvDbContext db;
        private readonly IBalancesService balanacesService;

        private readonly Mock<AbvInvestUser> moqUser;
        private readonly DateTime date = new DateTime(2018, 12, 20);

        public BalancesServiceTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV")
                .Options;
            db = new AbvDbContext(options);

            AutoMapperConfig.RegisterMappings(
                typeof(BalanceDto).Assembly);

            balanacesService = new BalancesService(db);

            moqUser = new Mock<AbvInvestUser>();
            moqUser.Setup(u => u.Balances).Returns(new HashSet<DailyBalance>());
            moqUser.Setup(u => u.Portfolio).Returns(new HashSet<DailySecuritiesPerClient> { new DailySecuritiesPerClient
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
                        ProfitPercentàge = 100,
                        PortfolioShare = 10
                    }
                }
            }});
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldNotCreateSecondDailyBalanceForUserForSameDate()
        {
            // Arrange
            await balanacesService.CreateBalanceForUser(moqUser.Object, date);
            var expectedUserBalancesCount = 1;

            // Act
            await balanacesService.CreateBalanceForUser(moqUser.Object, date);
            var actualUserBalancesCount = moqUser.Object.Balances.Count(b => b.Date == date);

            // Assert
            Assert.Equal(expectedUserBalancesCount, actualUserBalancesCount);
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldCreateDailyBalanceForUser()
        {
            // Arrange
            var expectedUserBalancesCount = 1;

            // Act
            await balanacesService.CreateBalanceForUser(moqUser.Object, date);
            var actualUserBalancesCount = moqUser.Object.Balances.Count;

            // Assert
            Assert.Equal(expectedUserBalancesCount, actualUserBalancesCount);
            Assert.Contains(moqUser.Object.Balances, b => b.Date == date);
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldCreateBalanceForUser()
        {
            // Act
            await balanacesService.CreateBalanceForUser(moqUser.Object, date);

            // Assert
            Assert.NotNull(moqUser.Object.Balances.SingleOrDefault(b => b.Date == date)?.Balance);
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldReturnBalanceWithCorrectVirtualProfit()
        {
            // Arrange
            var expectedVirtualProfit = 10000;

            // Act
            await balanacesService.CreateBalanceForUser(moqUser.Object, date);
            var balance = moqUser.Object.Balances.SingleOrDefault(b => b.Date == date)?.Balance;
            var actualVirtualProfit = balance?.VirtualProfit;

            // Assert
            Assert.Equal(expectedVirtualProfit, actualVirtualProfit);
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldReturnBalanceWithCorrectVirtualProfitPercentage()
        {
            // Arrange
            var expectedVirtualProfitPercentage = 100;

            // Act
            await balanacesService.CreateBalanceForUser(moqUser.Object, date);
            var balance = moqUser.Object.Balances.SingleOrDefault(b => b.Date == date)?.Balance;
            var actualVirtualProfitPercentage = balance?.VirtualProfitPercentage;

            // Assert
            Assert.Equal(expectedVirtualProfitPercentage, actualVirtualProfitPercentage);
        }

        [Fact]
        public async Task GetUserDailyBalance_ShouldReturnBalance()
        {
            // Arrange
            await balanacesService.CreateBalanceForUser(moqUser.Object, date);

            // Act
            var actualUserBalance = balanacesService.GetUserDailyBalance<BalanceDto>(moqUser.Object, date);

            // Assert
            Assert.NotNull(actualUserBalance);
        }

        [Fact]
        public async Task GetUserDailyBalance_ShouldReturnBalanceWithCorrectProfitFigures()
        {
            // Arrange
            await balanacesService.CreateBalanceForUser(moqUser.Object, date);
            var expectedUserBalance = new BalanceDto
            {
                VirtualProfit = 10000,
                VirtualProfitPercentage = 100,
            };

            // Act
            var actualUserBalance = balanacesService.GetUserDailyBalance<BalanceDto>(moqUser.Object, date);

            // Assert
            Assert.Equal(expectedUserBalance.VirtualProfit, actualUserBalance.VirtualProfit);
            Assert.Equal(expectedUserBalance.VirtualProfitPercentage, actualUserBalance.VirtualProfitPercentage);
        }

        [Fact]
        public void GetUserDailyBalance_ShouldNotReturnBalanceIfSuchDoesNotExist()
        {
            // Act
            var actualUserBalance = balanacesService.GetUserDailyBalance<BalanceDto>(moqUser.Object, date);

            // Assert
            Assert.Null(actualUserBalance);
        }
    }
}
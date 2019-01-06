namespace ABV_Invest.Services.Tests
{
    using Contracts;
    using Data;
    using DTOs;
    using Mapping;
    using Models;

    using Microsoft.EntityFrameworkCore;
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
            this.db = new AbvDbContext(options);

            AutoMapperConfig.RegisterMappings(
                typeof(BalanceDto).Assembly);

            this.balanacesService = new BalancesService(this.db);

            this.moqUser = new Mock<AbvInvestUser>();
            this.moqUser.Setup(u => u.Balances).Returns(new HashSet<DailyBalance>());
            this.moqUser.Setup(u => u.Portfolio).Returns(new HashSet<DailySecuritiesPerClient> { new DailySecuritiesPerClient
            {
                Date = this.date,
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
            await this.balanacesService.CreateBalanceForUser(this.moqUser.Object, this.date);
            var expectedUserBalancesCount = 1;

            // Act
            await this.balanacesService.CreateBalanceForUser(this.moqUser.Object, this.date);
            var actualUserBalancesCount = this.moqUser.Object.Balances.Count(b => b.Date == this.date);

            // Assert
            Assert.Equal(expectedUserBalancesCount, actualUserBalancesCount);
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldCreateDailyBalanceForUser()
        {
            // Arrange
            var expectedUserBalancesCount = 1;

            // Act
            await this.balanacesService.CreateBalanceForUser(this.moqUser.Object, this.date);
            var actualUserBalancesCount = this.moqUser.Object.Balances.Count;

            // Assert
            Assert.Equal(expectedUserBalancesCount, actualUserBalancesCount);
            Assert.Contains(this.moqUser.Object.Balances, b => b.Date == this.date);
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldCreateBalanceForUser()
        {
            // Act
            await this.balanacesService.CreateBalanceForUser(this.moqUser.Object, this.date);

            // Assert
            Assert.NotNull(this.moqUser.Object.Balances.SingleOrDefault(b => b.Date == this.date)?.Balance);
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldReturnBalanceWithCorrectVirtualProfit()
        {
            // Arrange
            var expectedVirtualProfit = 10000;

            // Act
            await this.balanacesService.CreateBalanceForUser(this.moqUser.Object, this.date);
            var balance = this.moqUser.Object.Balances.SingleOrDefault(b => b.Date == this.date)?.Balance;
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
            await this.balanacesService.CreateBalanceForUser(this.moqUser.Object, this.date);
            var balance = this.moqUser.Object.Balances.SingleOrDefault(b => b.Date == this.date)?.Balance;
            var actualVirtualProfitPercentage = balance?.VirtualProfitPercentage;

            // Assert
            Assert.Equal(expectedVirtualProfitPercentage, actualVirtualProfitPercentage);
        }

        [Fact]
        public async Task GetUserDailyBalance_ShouldReturnBalance()
        {
            // Arrange
            await this.balanacesService.CreateBalanceForUser(this.moqUser.Object, this.date);

            // Act
            var actualUserBalance = this.balanacesService.GetUserDailyBalance<BalanceDto>(this.moqUser.Object, this.date);

            // Assert
            Assert.NotNull(actualUserBalance);
        }

        [Fact]
        public async Task GetUserDailyBalance_ShouldReturnBalanceWithCorrectProfitFigures()
        {
            // Arrange
            await this.balanacesService.CreateBalanceForUser(this.moqUser.Object, this.date);
            var expectedUserBalance = new BalanceDto
            {
                VirtualProfit = 10000,
                VirtualProfitPercentage = 100,
            };

            // Act
            var actualUserBalance = this.balanacesService.GetUserDailyBalance<BalanceDto>(this.moqUser.Object, this.date);

            // Assert
            Assert.Equal(expectedUserBalance.VirtualProfit, actualUserBalance.VirtualProfit);
            Assert.Equal(expectedUserBalance.VirtualProfitPercentage, actualUserBalance.VirtualProfitPercentage);
        }

        [Fact]
        public void GetUserDailyBalance_ShouldNotReturnBalanceIfSuchDoesNotExist()
        {
            // Act
            var actualUserBalance = this.balanacesService.GetUserDailyBalance<BalanceDto>(this.moqUser.Object, this.date);

            // Assert
            Assert.Null(actualUserBalance);
        }
    }
}
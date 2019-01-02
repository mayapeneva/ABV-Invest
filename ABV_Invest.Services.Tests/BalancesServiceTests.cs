namespace ABV_Invest.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Data;
    using DTOs;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Moq;
    using Xunit;

    public class BalancesServiceTests
    {
        private readonly AbvDbContext Db;
        private readonly IBalancesService BalanacesService;
        private readonly Mock<AbvInvestUser> MoqUser;
        private readonly DateTime Date = new DateTime(2018, 12, 28);

        public BalancesServiceTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV")
                .Options;
            this.Db = new AbvDbContext(options);

            AutoMapperConfig.RegisterMappings(
                typeof(BalanceDto).Assembly);

            this.BalanacesService = new BalancesService(this.Db);

            var date = new DateTime(2018, 12, 28);
            this.MoqUser = new Mock<AbvInvestUser>();
            this.MoqUser.Setup(u => u.Balances).Returns(new HashSet<DailyBalance>());
            this.MoqUser.Setup(u => u.Portfolio).Returns(new HashSet<DailySecuritiesPerClient> { new DailySecuritiesPerClient
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
                }}});
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldCreateDailyBalanceForUser()
        {
            // Arrange
            var expectedUserBalancesCount = 1;

            // Act
            await this.BalanacesService.CreateBalanceForUser(this.MoqUser.Object, this.Date);
            var actualUserBalancesCount = this.MoqUser.Object.Balances.Count;

            // Assert
            Assert.Equal(expectedUserBalancesCount, actualUserBalancesCount);
            Assert.Contains(this.MoqUser.Object.Balances, b => b.Date == this.Date);
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldCreateBalanceForUser()
        {
            // Act
            await this.BalanacesService.CreateBalanceForUser(this.MoqUser.Object, this.Date);

            // Assert
            Assert.NotNull(this.MoqUser.Object.Balances.SingleOrDefault(b => b.Date == this.Date)?.Balance);
        }

        [Fact]
        public async Task CreateBalanceForUser_ShouldReturnBalanceWithCorrectVirtualProfit()
        {
            // Arrange
            var expectedVirtualProfit = 10000;

            // Act
            await this.BalanacesService.CreateBalanceForUser(this.MoqUser.Object, this.Date);
            var balance = this.MoqUser.Object.Balances.SingleOrDefault(b => b.Date == this.Date)?.Balance;
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
            await this.BalanacesService.CreateBalanceForUser(this.MoqUser.Object, this.Date);
            var balance = this.MoqUser.Object.Balances.SingleOrDefault(b => b.Date == this.Date)?.Balance;
            var actualVirtualProfitPercentage = balance?.VirtualProfitPercentage;

            // Assert
            Assert.Equal(expectedVirtualProfitPercentage, actualVirtualProfitPercentage);
        }

        [Fact]
        public async Task GetUserDailyBalance_ShouldReturnBalance()
        {
            // Arrange
            await this.BalanacesService.CreateBalanceForUser(this.MoqUser.Object, this.Date);

            // Act
            var actualUserBalance = this.BalanacesService.GetUserDailyBalance<BalanceDto>(this.MoqUser.Object, this.Date);

            // Assert
            Assert.NotNull(actualUserBalance);
        }

        [Fact]
        public async Task GetUserDailyBalance_ShouldReturnBalanceWithCorrectProfitFigures()
        {
            // Arrange
            await this.BalanacesService.CreateBalanceForUser(this.MoqUser.Object, this.Date);
            var expectedUserBalance = new BalanceDto
            {
                AllSecuritiesTotalPriceBuy = 10000,
                AllSecuritiesTotalMarketPrice = 20000,
                VirtualProfit = 10000,
                VirtualProfitPercentage = 100,
                CurrencyCode = "BGN"
            };

            // Act
            var actualUserBalance = this.BalanacesService.GetUserDailyBalance<BalanceDto>(this.MoqUser.Object, this.Date);

            // Assert
            Assert.Equal(expectedUserBalance.VirtualProfit, actualUserBalance.VirtualProfit);
            Assert.Equal(expectedUserBalance.VirtualProfitPercentage, actualUserBalance.VirtualProfitPercentage);
        }

        [Fact]
        public void GetUserDailyBalance_ShouldNotReturnBalanceIfSuchDoesNotExist()
        {
            // Act
            var actualUserBalance = this.BalanacesService.GetUserDailyBalance<BalanceDto>(this.MoqUser.Object, this.Date);

            // Assert
            Assert.Null(actualUserBalance);
        }
    }
}
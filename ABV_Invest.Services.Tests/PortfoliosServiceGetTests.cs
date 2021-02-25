﻿namespace ABV_Invest.Services.Tests
{
    using Contracts;
    using Data;
    using DTOs;
    using Mapping;
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

        private DateTime Date = new DateTime(2018, 12, 15);

        public PortfoliosServiceGetTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV")
                .Options;
            this.db = new AbvDbContext(options);

            AutoMapperConfig.RegisterMappings(
                typeof(PortfolioDto).Assembly);

            this.moqUser = new Mock<AbvInvestUser>();
            this.moqUser.Setup(u => u.Portfolio).Returns(new HashSet<DailySecuritiesPerClient> { new DailySecuritiesPerClient
            {
                Date = this.Date,
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

            var balancesService = new BalancesService(this.db);
            var dataService = new DataService(this.db);
            var mockUserStore = new Mock<IUserStore<AbvInvestUser>>();
            var userManager = new Mock<UserManager<AbvInvestUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            this.principal = new ClaimsPrincipal();
            userManager.Setup(um => um.GetUserAsync(this.principal)).Returns(Task.FromResult(this.moqUser.Object));
            this.portfoliosService = new PortfoliosService(this.db, userManager.Object, balancesService, dataService);
        }

        [Fact]
        public void GetUserDailyPortfolio_ShouldReturnDailyPortfolio()
        {
            // Act
            var result = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.principal, this.Date);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetUserDailyPortfolio_ShouldReturnDailyPortfolioWithCorrectTotalMarketPrice()
        {
            // Arange
            var expectedTotalMarketPrice =
                this.moqUser.Object.Portfolio.Select(p => p.SecuritiesPerIssuerCollection.Sum(s => s.TotalMarketPrice));

            // Act
            var totalMarketPrice = await this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.principal, this.Date);
            var actualTotalMarketPrice = totalMarketPrice.Select(p => p.TotalMarketPrice);

            // Assert
            Assert.Equal(expectedTotalMarketPrice, actualTotalMarketPrice);
        }

        [Fact]
        public void GetUserDailyPortfolio_ShouldReturnNullIfThereIsNoPortfolioForThisDate()
        {
            // Arange
            var date = new DateTime(2018, 12, 27);

            // Act
            var result = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.principal, date);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserDailyPortfolio_ShouldReturnNullIfThereIsNoSuchUser()
        {
            // Arange
            var user = new ClaimsPrincipal();

            // Act
            var result = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(user, this.Date);

            // Assert
            Assert.Null(result);
        }
    }
}
namespace ABV_Invest.Services.Tests
{
    using ABV_Invest.Common.DTOs;
    using ABV_Invest.Common.Mapping;
    using Contracts;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Models.Enums;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Xunit;

    public class DealsServiceGetTests
    {
        private readonly AbvDbContext db;
        private readonly IDealsService dealsService;
        private readonly Mock<AbvInvestUser> moqUser;
        private readonly ClaimsPrincipal principal;

        private DateTime Date = new DateTime(2020, 12, 16);

        public DealsServiceGetTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABVInvest")
                .Options;
            db = new AbvDbContext(options);

            AutoMapperConfig.RegisterMappings(
                typeof(DealDto).Assembly);

            moqUser = new Mock<AbvInvestUser>();
            moqUser.Setup(u => u.Deals).Returns(new HashSet<DailyDeals> { new DailyDeals
            {
                Date = Date,
                Deals = new HashSet<Deal> { new Deal
                    {
                        DealType = DealType.Купува,
                        Quantity = 100,
                        Price = 100,
                        Coupon = 0,
                        TotalPrice = 10000,
                        Fee = 90,
                        Settlement = new DateTime(2020, 12, 18)
                    }
                }
            }});

            var mockUserStore = new Mock<IUserStore<AbvInvestUser>>();
            var userManager = new Mock<UserManager<AbvInvestUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            principal = new ClaimsPrincipal();
            userManager.Setup(um => um.GetUserAsync(principal)).Returns(Task.FromResult(moqUser.Object));
            var dataService = new DataService(db);
            dealsService = new DealsService(db, userManager.Object, dataService);
        }

        [Fact]
        public void GetUserDailyDeals_ShouldReturnDailyDeals()
        {
            // Act
            var result = dealsService.GetUserDailyDeals<DealDto>(principal, Date);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetUserDailyDeals_ShouldReturnDailyDealsWithCorrectTotalPrice()
        {
            // Arange
            var expectedTotalPrice =
                moqUser.Object.Deals.Select(dd => dd.Deals.Sum(d => d.TotalPrice));

            // Act
            var totalPrice = await dealsService.GetUserDailyDeals<DealDto>(principal, Date);
            var actualTotalPrice = totalPrice.Select(d => d.TotalPrice);

            // Assert
            Assert.Equal(expectedTotalPrice, actualTotalPrice);
        }

        [Fact]
        public async Task GetUserDailyDeals_ShouldReturnNullIfThereIsNoDealsForThisDate()
        {
            // Arange
            var date = new DateTime(2020, 12, 27);

            // Act
            var result = await dealsService.GetUserDailyDeals<DealDto>(principal, date);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserDailyDeals_ShouldReturnNullIfThereIsNoSuchUser()
        {
            // Arange
            var user = new ClaimsPrincipal();

            // Act
            var result = await dealsService.GetUserDailyDeals<DealDto>(user, Date);

            // Assert
            Assert.Null(result);
        }
    }
}
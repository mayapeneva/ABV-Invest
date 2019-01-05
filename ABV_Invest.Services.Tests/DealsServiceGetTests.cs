﻿namespace ABV_Invest.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Contracts;
    using Data;
    using DTOs;
    using Mapping;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Models.Enums;
    using Moq;
    using Xunit;

    public class DealsServiceGetTests
    {
        private const string Date = "16/12/2018";

        private readonly AbvDbContext db;
        private readonly IDealsService dealsService;
        private readonly Mock<AbvInvestUser> moqUser;
        private readonly ClaimsPrincipal principal;

        public DealsServiceGetTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV")
                .Options;
            this.db = new AbvDbContext(options);

            AutoMapperConfig.RegisterMappings(
                typeof(DealDto).Assembly);

            var date = new DateTime(2018, 12, 16);
            this.moqUser = new Mock<AbvInvestUser>();
            this.moqUser.Setup(u => u.Deals).Returns(new HashSet<DailyDeals> { new DailyDeals
            {
                Date = date,
                Deals = new HashSet<Deal> { new Deal
                    {
                        DealType = DealType.Купува,
                        Quantity = 100,
                        Price = 100,
                        Coupon = 0,
                        TotalPrice = 10000,
                        Fee = 90,
                        Settlement = new DateTime(2018, 12, 18)
                    }
                }
            }});

            var mockUserStore = new Mock<IUserStore<AbvInvestUser>>();
            var userManager = new Mock<UserManager<AbvInvestUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            this.principal = new ClaimsPrincipal();
            userManager.Setup(um => um.GetUserAsync(this.principal)).Returns(Task.FromResult(this.moqUser.Object));
            this.dealsService = new DealsService(this.db, userManager.Object);
        }

        [Fact]
        public void GetUserDailyDeals_ShouldReturnDailyDeals()
        {
            // Act
            var result = this.dealsService.GetUserDailyDeals<DealDto>(this.principal, Date);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetUserDailyDeals_ShouldReturnDailyDealsWithCorrectTotalPrice()
        {
            // Arange
            var expectedTotalPrice =
                this.moqUser.Object.Deals.Select(dd => dd.Deals.Sum(d => d.TotalPrice));

            // Act
            var actualTotalPrice = this.dealsService.GetUserDailyDeals<DealDto>(this.principal, Date).Select(d => d.TotalPrice);

            // Assert
            Assert.Equal(expectedTotalPrice, actualTotalPrice);
        }

        [Fact]
        public void GetUserDailyDeals_ShouldReturnNullIfThereIsNoDealsForThisDate()
        {
            // Arange
            var date = "27/12/2018";

            // Act
            var result = this.dealsService.GetUserDailyDeals<DealDto>(this.principal, date);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserDailyDeals_ShouldReturnNullIfThereIsNoSuchUser()
        {
            // Arange
            var user = new ClaimsPrincipal();

            // Act
            var result = this.dealsService.GetUserDailyDeals<DealDto>(user, Date);

            // Assert
            Assert.Null(result);
        }
    }
}
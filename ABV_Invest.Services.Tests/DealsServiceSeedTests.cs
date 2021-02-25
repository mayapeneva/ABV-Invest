namespace ABV_Invest.Services.Tests
{
    using BindingModels.Uploads.Deals;
    using Contracts;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Moq;
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Xunit;

    public class DealsServiceSeedTests
    {
        private const string UserNameOne = "V000017601";
        private const string UserNameTwo = "V000018024";

        private readonly AbvDbContext db;
        private readonly IDealsService dealsService;

        private readonly DealRowBindingModel[] deserializedDeals;

        public DealsServiceSeedTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABVInvest").Options;
            this.db = new AbvDbContext(options);

            if (!this.db.AbvInvestUsers.Any(u => u.UserName == UserNameOne))
            {
                this.db.AbvInvestUsers.Add(new AbvInvestUser
                {
                    UserName = UserNameOne
                });
                this.db.SaveChanges();
            }

            if (!this.db.AbvInvestUsers.Any(u => u.UserName == UserNameTwo))
            {
                this.db.AbvInvestUsers.Add(new AbvInvestUser
                {
                    UserName = UserNameTwo
                });
                this.db.SaveChanges();
            }

            if (!this.db.Markets.Any(m => m.MIC == "XBUL"))
            {
                var market = new Market
                {
                    Name = "БФБ",
                    MIC = "XBUL"
                };
                this.db.Markets.Add(market);
                this.db.SaveChanges();
            }

            var mockUserStore = new Mock<IUserStore<AbvInvestUser>>();
            var userManager = new Mock<UserManager<AbvInvestUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            var moqUser = new Mock<AbvInvestUser>();
            userManager.Setup(um => um.GetUserAsync(new ClaimsPrincipal())).Returns(Task.FromResult(moqUser.Object));
            var dataService = new DataService(this.db);
            this.dealsService = new DealsService(this.db, userManager.Object, dataService);

            var fileName = "../../../Files/Deals/Deals.xml";
            var xmlFileContent = File.ReadAllText(fileName);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            this.deserializedDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));
        }

        [Fact]
        public async Task _1_SeedDeals_ShouldCreateDealCollectionForThisDateForEachUserInFile()
        {
            // Arrange
            var date = new DateTime(2018, 11, 01);
            var expected = true;

            // Act
            await this.dealsService.SeedDeals(this.deserializedDeals, date);
            var actualUser1 = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Deals.Any(p => p.Date == date);
            var actualUser2 = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Deals.Any(p => p.Date == date);

            // Assert
            Assert.Equal(expected, actualUser1);
            Assert.Equal(expected, actualUser2);
        }

        [Fact]
        public async Task _2_SeedDeals__ShouldNotCreateDealCollectionForNonExistingUser()
        {
            // Arrange
            var fileName2 = "../../../Files/Deals/Deals2.xml";
            var xmlFileContent = File.ReadAllText(fileName2);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 02);
            var userName = "V000018048";

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var usersDeals = this.db.DailyDeals.SingleOrDefault(dd => dd.AbvInvestUser.UserName == userName);

            // Assert
            Assert.Null(usersDeals);
        }

        [Fact]
        public async Task _3_SeedDeals_ShouldNotCreateSecondDealsCollectionForUserWithSameDate()
        {
            // Arange
            var date = new DateTime(2018, 12, 03);
            await this.dealsService.SeedDeals(this.deserializedDeals, date);
            var expectedDealsCount = 1;

            // Act
            await this.dealsService.SeedDeals(this.deserializedDeals, date);
            var actualDealsCount = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Deals.Count(p => p.Date == date);

            // Assert
            Assert.Equal(expectedDealsCount, actualDealsCount);
        }

        [Fact]
        public async Task _4_SeedDeals_ShouldCreateSecurityIfItDoesNotExist()
        {
            // Arrange
            var fileName3 = "../../../Files/Deals/Deals3.xml";
            var xmlFileContent = File.ReadAllText(fileName3);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 04);
            var securityISIN = "BG1100041067";

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var securities = this.db.Securities;

            // Assert
            Assert.Contains(securities, s => s.ISIN == securityISIN);
        }

        [Fact]
        public async Task _5_SeedDeals_ShouldNotCreateEntryIfSecurityHasWrongISIN()
        {
            // Arrange
            var fileName4 = "../../../Files/Deals/Deals4.xml";
            var xmlFileContent = File.ReadAllText(fileName4);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 05);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task _6_SeedDeals_ShouldCreateCurrencyIfItDoesNotExist()
        {
            // Arrange
            var fileName5 = "../../../Files/Deals/Deals5.xml";
            var xmlFileContent = File.ReadAllText(fileName5);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 06);
            var currencyCode = "EUR";

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var currencies = this.db.Currencies;

            // Assert
            Assert.Contains(currencies, c => c.Code == currencyCode);
        }

        [Fact]
        public async Task _7_SeedDeals_ShouldNotCreateEntryIfCurrencyHasWrongCode()
        {
            // Arrange
            var fileName6 = "../../../Files/Deals/Deals6.xml";
            var xmlFileContent = File.ReadAllText(fileName6);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 07);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task _8_SeedDeals_ShouldNotCreateEntryIfMarketDoesNotExist()
        {
            // Arrange
            var fileName7 = "../../../Files/Deals/Deals7.xml";
            var xmlFileContent = File.ReadAllText(fileName7);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 08);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task _9_SeedDeals_ShouldNotCreateEntryIfDealTypeNotValid()
        {
            // Arrange
            var fileName8 = "../../../Files/Deals/Deals8.xml";
            var xmlFileContent = File.ReadAllText(fileName8);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 09);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task _10_SeedDeals_ShouldNotCreateEntryIfSettlementDateNotValid()
        {
            // Arrange
            var fileName9 = "../../../Files/Deals/Deals9.xml";
            var xmlFileContent = File.ReadAllText(fileName9);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 10);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task _11_SeedDeals_ShouldNotCreateEntryIfSinglePriceCoultNotBeParsed()
        {
            // Arrange
            var fileName10 = "../../../Files/Deals/Deals10.xml";
            var xmlFileContent = File.ReadAllText(fileName10);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 11);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task _12_SeedDeals_ShouldNotCreateEntryIfShareCountCoultNotBeParsed()
        {
            // Arrange
            var fileName11 = "../../../Files/Deals/Deals11.xml";
            var xmlFileContent = File.ReadAllText(fileName11);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 12);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task _13_SeedDeals_ShouldNotCreateEntryIfCouponCoultNotBeParsed()
        {
            // Arrange
            var fileName12 = "../../../Files/Deals/Deals12.xml";
            var xmlFileContent = File.ReadAllText(fileName12);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 13);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task _14_SeedDeals_ShouldNotCreateEntryIfAmountInShareCurrencyCoultNotBeParsed()
        {
            // Arrange
            var fileName13 = "../../../Files/Deals/Deals13.xml";
            var xmlFileContent = File.ReadAllText(fileName13);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 14);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task _15_SeedDeals_ShouldNotCreateEntryIfAmountInPaymentCurrencyCoultNotBeParsed()
        {
            // Arrange
            var fileName14 = "../../../Files/Deals/Deals14.xml";
            var xmlFileContent = File.ReadAllText(fileName14);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 15);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }

        [Fact]
        public async Task _16_SeedDeals_ShouldNotCreateEntryIfCommissionCoultNotBeParsed()
        {
            // Arrange
            var fileName15 = "../../../Files/Deals/Deals15.xml";
            var xmlFileContent = File.ReadAllText(fileName15);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 16);

            // Act
            await this.dealsService.SeedDeals(deserDeals, date);
            var dailyDeals = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Deals.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyDeals);
        }
    }
}
namespace ABV_Invest.Services.Tests
{
    using ABV_Invest.Common.BindingModels.Uploads.Portfolios;
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

    public class PortfoliosServiceSeedTests
    {
        private const string UserNameOne = "0000000001";
        private const string UserNameTwo = "0000000003";

        private readonly AbvDbContext db;
        private readonly IPortfoliosService portfoliosService;

        private readonly PortfolioRowBindingModel[] deserializedPortfolios;

        public PortfoliosServiceSeedTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV").Options;
            db = new AbvDbContext(options);

            if (!db.AbvInvestUsers.Any(u => u.UserName == UserNameOne))
            {
                db.AbvInvestUsers.Add(new AbvInvestUser
                {
                    UserName = UserNameOne
                });
                db.SaveChanges();
            }

            if (!db.AbvInvestUsers.Any(u => u.UserName == UserNameTwo))
            {
                db.AbvInvestUsers.Add(new AbvInvestUser
                {
                    UserName = UserNameTwo
                });
                db.SaveChanges();
            }

            var mockUserStore = new Mock<IUserStore<AbvInvestUser>>();
            var userManager = new Mock<UserManager<AbvInvestUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            var moqUser = new Mock<AbvInvestUser>();
            userManager.Setup(um => um.GetUserAsync(new ClaimsPrincipal())).Returns(Task.FromResult(moqUser.Object));
            var balancesService = new BalancesService(db);
            var dataService = new DataService(db);
            portfoliosService = new PortfoliosService(db, userManager.Object, balancesService, dataService);

            var fileName = "../../../Files/Portfolios/Portfolios.xml";
            var xmlFileContent = File.ReadAllText(fileName);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            this.deserializedPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));
        }

        [Fact]
        public async Task _1_SeedPortfolios_ShouldCreatePortfolioForThisDateForEachUserInFile()
        {
            // Arrange
            var date = new DateTime(2020, 12, 01);
            var expected = true;

            // Act
            await portfoliosService.SeedPortfolios(deserializedPortfolios, date);
            var actualUser1 = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Portfolio.Any(p => p.Date == date);
            var actualUser2 = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.Any(p => p.Date == date); ;

            // Assert
            Assert.Equal(expected, actualUser1);
            Assert.Equal(expected, actualUser2);
        }

        [Fact]
        public async Task _2_SeedPortfolios_ShouldNotCreatePortfolioForNonExistingUser()
        {
            // Arrange
            var fileName2 = "../../../Files/Portfolios/Portfolios2.xml";
            var xmlFileContent = File.ReadAllText(fileName2);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 02);
            var userName = "0000000002";

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var usersPortfolio = db.DailySecuritiesPerClient.SingleOrDefault(ds => ds.AbvInvestUser.UserName == userName);

            // Assert
            Assert.Null(usersPortfolio);
        }

        [Fact]
        public async Task _3_SeedPortfolios_ShouldNotCreateSecondProtfolioForUserWithSameDate()
        {
            // Arange
            var date = new DateTime(2020, 12, 03);
            await portfoliosService.SeedPortfolios(deserializedPortfolios, date);
            var expectedPortfoliosCount = 1;

            // Act
            await portfoliosService.SeedPortfolios(deserializedPortfolios, date);
            var actualPortfoliosCount = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Portfolio.Count(p => p.Date == date);

            // Assert
            Assert.Equal(expectedPortfoliosCount, actualPortfoliosCount);
        }

        [Fact]
        public async Task _4_SeedPortfolios_ShouldAddUsersFullNameIfThereIsNone()
        {
            // Arange
            var userName = "0000000008";
            db.AbvInvestUsers.Add(new AbvInvestUser
            {
                UserName = userName
            });
            db.SaveChanges();

            var fileName3 = "../../../Files/Portfolios/Portfolios3.xml";
            var xmlFileContent = File.ReadAllText(fileName3);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 04);
            var expectedUserFullName = "ИНДЪСТРИ ДИВЕЛЪПМЪНТ ХОЛДИНГ АД";

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var user = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == userName);

            // Assert
            Assert.Equal(expectedUserFullName, user?.FullName);
        }

        [Fact]
        public async Task _5_SeedPortfolios_ShouldCreateSecurityIfItDoesNotExist()
        {
            // Arange
            var fileName4 = "../../../Files/Portfolios/Portfolios4.xml";
            var xmlFileContent = File.ReadAllText(fileName4);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 05);
            var securityISIN = "BG1100019980";

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var securities = db.Securities;

            // Assert
            Assert.Contains(securities, s => s.ISIN == securityISIN);
        }

        [Fact]
        public async Task _6_SeedPortfolios_ShouldNotCreateEntryIfSecurityHasWrongISIN()
        {
            // Arange
            var fileName5 = "../../../Files/Portfolios/Portfolios5.xml";
            var xmlFileContent = File.ReadAllText(fileName5);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 06);

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _7_SeedPortfolios_ShouldNotMakePortfolioRowEntryIfSecurityAlreadyEnteredInThisDailyPortfolio()
        {
            // Arange
            var date = new DateTime(2020, 12, 07);
            var securityISIN = "BG1100026985";
            var expectedSecurityEntryCount = 1;

            // Act
            await portfoliosService.SeedPortfolios(deserializedPortfolios, date);
            var actualSecurityEntryCount = (db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Portfolio.SingleOrDefault(p => p.Date == date)?.SecuritiesPerIssuerCollection.Select(s => s.Security.ISIN))?.Count(n => n == securityISIN);

            // Assert
            Assert.Equal(expectedSecurityEntryCount, actualSecurityEntryCount);
        }

        [Fact]
        public async Task _8_SeedPortfolios_ShouldCreateCurrencyIfItDoesNotExist()
        {
            // Arange
            var fileName6 = "../../../Files/Portfolios/Portfolios6.xml";
            var xmlFileContent = File.ReadAllText(fileName6);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 08);
            var currencyCode = "EUR";

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var currencies = db.Currencies;

            // Assert
            Assert.Contains(currencies, c => c.Code == currencyCode);
        }

        [Fact]
        public async Task _9_SeedPortfolios_ShouldNotCreateEntryIfCurrencyHasWrongCode()
        {
            // Arange
            var fileName7 = "../../../Files/Portfolios/Portfolios7.xml";
            var xmlFileContent = File.ReadAllText(fileName7);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 09);

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _10_SeedPortfolios_ShouldNotCreateEntryIfQuantityCoultNotBeParsed()
        {
            // Arange
            var fileName8 = "../../../Files/Portfolios/Portfolios8.xml";
            var xmlFileContent = File.ReadAllText(fileName8);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 10);

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _11_SeedPortfolios_ShouldNotCreateEntryIfOpenPriceCoultNotBeParsed()
        {
            // Arange
            var fileName9 = "../../../Files/Portfolios/Portfolios9.xml";
            var xmlFileContent = File.ReadAllText(fileName9);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 11);

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _12_SeedPortfolios_ShouldNotCreateEntryIfMarketPriceCoultNotBeParsed()
        {
            // Arange
            var fileName10 = "../../../Files/Portfolios/Portfolios10.xml";
            var xmlFileContent = File.ReadAllText(fileName10);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 12);

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _13_SeedPortfolios_ShouldNotCreateEntryIfMarketValueCoultNotBeParsed()
        {
            // Arange
            var fileName11 = "../../../Files/Portfolios/Portfolios11.xml";
            var xmlFileContent = File.ReadAllText(fileName11);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 13);

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _14_SeedPortfolios_ShouldNotCreateEntryIfResultCoultNotBeParsed()
        {
            // Arange
            var fileName12 = "../../../Files/Portfolios/Portfolios12.xml";
            var xmlFileContent = File.ReadAllText(fileName12);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 14);

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _15_SeedPortfolios_ShouldNotCreateEntryIfResultBGNCoultNotBeParsed()
        {
            // Arange
            var fileName13 = "../../../Files/Portfolios/Portfolios13.xml";
            var xmlFileContent = File.ReadAllText(fileName13);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 15);

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _16_SeedPortfolios_ShouldNotCreateEntryIfYieldPercentCoultNotBeParsed()
        {
            // Arange
            var fileName14 = "../../../Files/Portfolios/Portfolios14.xml";
            var xmlFileContent = File.ReadAllText(fileName14);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 16);

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _17_SeedPortfolios_ShouldNotCreateEntryIfRelativePartCoultNotBeParsed()
        {
            // Arange
            var fileName15 = "../../../Files/Portfolios/Portfolios15.xml";
            var xmlFileContent = File.ReadAllText(fileName15);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2020, 12, 17);

            // Act
            await portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }
    }
}
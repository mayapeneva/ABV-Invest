namespace ABV_Invest.Services.Tests
{
    using BindingModels.Uploads.Portfolios;
    using Contracts;
    using Data;
    using Models;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
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
        private const string UserNameOne = "V000017601";
        private const string UserNameTwo = "V000018024";

        private readonly AbvDbContext db;
        private readonly IPortfoliosService portfoliosService;

        private readonly PortfolioRowBindingModel[] deserializedPortfolios;

        public PortfoliosServiceSeedTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV").Options;
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

            var mockUserStore = new Mock<IUserStore<AbvInvestUser>>();
            var userManager = new Mock<UserManager<AbvInvestUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            var moqUser = new Mock<AbvInvestUser>();
            userManager.Setup(um => um.GetUserAsync(new ClaimsPrincipal())).Returns(Task.FromResult(moqUser.Object));
            var balancesService = new BalancesService(this.db);
            var dataService = new DataService(this.db);
            this.portfoliosService = new PortfoliosService(this.db, userManager.Object, balancesService, dataService);

            var fileName = "../../../Files/Portfolios/Portfolios.xml";
            var xmlFileContent = File.ReadAllText(fileName);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            this.deserializedPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));
        }

        [Fact]
        public async Task _1_SeedPortfolios_ShouldCreatePortfolioForThisDateForEachUserInFile()
        {
            // Arrange
            var date = new DateTime(2018, 12, 01);
            var expected = true;

            // Act
            await this.portfoliosService.SeedPortfolios(this.deserializedPortfolios, date);
            var actualUser1 = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Portfolio.Any(p => p.Date == date);
            var actualUser2 = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.Any(p => p.Date == date); ;

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

            var date = new DateTime(2018, 12, 02);
            var userName = "V000018048";

            // Act
            await this.portfoliosService.SeedPortfolios(deserPortfolios, date);
            var usersPortfolio = this.db.DailySecuritiesPerClient.SingleOrDefault(ds => ds.AbvInvestUser.UserName == userName);

            // Assert
            Assert.Null(usersPortfolio);
        }

        [Fact]
        public async Task _3_SeedPortfolios_ShouldNotCreateSecondProtfolioForUserWithSameDate()
        {
            // Arange
            var date = new DateTime(2018, 12, 03);
            await this.portfoliosService.SeedPortfolios(this.deserializedPortfolios, date);
            var expectedPortfoliosCount = 1;

            // Act
            await this.portfoliosService.SeedPortfolios(this.deserializedPortfolios, date);
            var actualPortfoliosCount = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Portfolio.Count(p => p.Date == date);

            // Assert
            Assert.Equal(expectedPortfoliosCount, actualPortfoliosCount);
        }

        [Fact]
        public async Task _4_SeedPortfolios_ShouldAddUsersFullNameIfThereIsNone()
        {
            // Arange
            var userName = "0000000008";
            this.db.AbvInvestUsers.Add(new AbvInvestUser
            {
                UserName = userName
            });
            this.db.SaveChanges();

            var fileName3 = "../../../Files/Portfolios/Portfolios3.xml";
            var xmlFileContent = File.ReadAllText(fileName3);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 04);
            var expectedUserFullName = "ИНДЪСТРИ ДИВЕЛЪПМЪНТ ХОЛДИНГ АД";

            // Act
            await this.portfoliosService.SeedPortfolios(deserPortfolios, date);
            var user = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == userName);

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

            var date = new DateTime(2018, 12, 05);
            var securityISIN = "BG1100019980";

            // Act
            await this.portfoliosService.SeedPortfolios(deserPortfolios, date);
            var securities = this.db.Securities;

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

            var date = new DateTime(2018, 12, 06);

            // Act
            await this.portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _7_SeedPortfolios_ShouldNotMakePortfolioRowEntryIfSecurityAlreadyEnteredInThisDailyPortfolio()
        {
            // Arange
            var date = new DateTime(2018, 12, 07);
            var securityISIN = "BG1100026985";
            var expectedSecurityEntryCount = 1;

            // Act
            await this.portfoliosService.SeedPortfolios(this.deserializedPortfolios, date);
            var actualSecurityEntryCount = (this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Portfolio.SingleOrDefault(p => p.Date == date)?.SecuritiesPerIssuerCollection.Select(s => s.Security.ISIN))?.Count(n => n == securityISIN);

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

            var date = new DateTime(2018, 12, 08);
            var currencyCode = "EUR";

            // Act
            await this.portfoliosService.SeedPortfolios(deserPortfolios, date);
            var currencies = this.db.Currencies;

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

            var date = new DateTime(2018, 12, 09);

            // Act
            await this.portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameOne)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }

        [Fact]
        public async Task _10_SeedPortfolios_ShouldNotCreateEntryIfAnyOfTheDecimalFiguresCoultNotBeParsed()
        {
            // Arange
            var fileName8 = "../../../Files/Portfolios/Portfolios8.xml";
            var xmlFileContent = File.ReadAllText(fileName8);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
            var deserPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

            var date = new DateTime(2018, 12, 10);

            // Act
            await this.portfoliosService.SeedPortfolios(deserPortfolios, date);
            var dailyPortfolio = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == UserNameTwo)?.Portfolio.SingleOrDefault(p => p.Date == date);

            // Assert
            Assert.Null(dailyPortfolio);
        }
    }
}
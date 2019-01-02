namespace ABV_Invest.Services.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Xunit;

    public class DataServiceTests
    {
        private readonly AbvDbContext Db;
        private readonly IDataService DataService;

        public DataServiceTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV")
                .Options;
            this.Db = new AbvDbContext(options);

            this.DataService = new DataService(this.Db);
        }

        [Fact]
        public async Task CreateCurrency_ShouldCreateCurrency()
        {
            // Arrange
            var currencyCode = "USD";
            var expectedCurrenciesCount = 1;

            // Act
            await this.DataService.CreateCurrency(currencyCode);
            var actualCurrenciesCount = this.Db.Currencies.Count();

            // Assert
            Assert.Equal(expectedCurrenciesCount, actualCurrenciesCount);
            Assert.Contains(this.Db.Currencies, c => c.Code == currencyCode);
        }

        [Fact]
        public async Task CreateCurrency_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            var currencyCode = "USD";
            await this.DataService.CreateCurrency(currencyCode);
            var expected = false;

            // Act
            var actual = this.DataService.CreateCurrency(currencyCode).GetAwaiter().GetResult();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateCurrency_ShouldNotCreateCurrencyIfCodeNotCorrect()
        {
            // Arrange
            var currencyCode = "US1";
            var expected = false;

            // Act
            var actual = this.DataService.CreateCurrency(currencyCode).GetAwaiter().GetResult();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateMarket_ShouldCreateMarket()
        {
            // Arrange
            var marketName = "БФБ";
            var marketCode = "XBUL";
            var expectedMarketsCount = 1;

            // Act
            await this.DataService.CreateMarket(marketName, marketCode);
            var actualMarketsCount = this.Db.Markets.Count();

            // Assert
            Assert.Equal(expectedMarketsCount, actualMarketsCount);
            Assert.Contains(this.Db.Markets, c => c.MIC == marketCode);
        }

        [Fact]
        public async Task CreateMarket_ShouldNotCreateMarketIfSuchAlreadyExists()
        {
            // Arrange
            var marketName = "БФБ";
            var marketCode = "XBUL";
            await this.DataService.CreateMarket(marketName, marketCode);
            var expected = false;

            // Act
            var actual = this.DataService.CreateMarket(marketName, marketName).GetAwaiter().GetResult();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateMarket_ShouldNotCreateMarketIfCodeNotCorrect()
        {
            // Arrange
            var marketName = "БФБ";
            var marketCode = "XBULL";
            var expected = false;

            // Act
            var actual = this.DataService.CreateMarket(marketName, marketCode).GetAwaiter().GetResult();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateSecurity_ShouldCreateSecurity()
        {
            // Arrange
            var issuerName = "БФБ";
            var ISIN = "BG1100008983";
            var bfbCode = "HSOF";
            var currencyCode = "USD";
            var expectedSecuritiesCount = 1;

            // Act
            await this.DataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);
            var actualSecuritiesCount = this.Db.Securities.Count();

            // Assert
            Assert.Equal(expectedSecuritiesCount, actualSecuritiesCount);
            Assert.Contains(this.Db.Securities, c => c.ISIN == ISIN);
            Assert.Contains(this.Db.Securities, c => c.BfbCode == bfbCode);
        }

        [Fact]
        public async Task CreateSecurity_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            var issuerName = "БФБ";
            var ISIN = "BG1100008983";
            var bfbCode = "HSOF";
            var currencyCode = "USD";
            await this.DataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);
            var expected = false;

            // Act
            var actual = this.DataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode).GetAwaiter().GetResult();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateSecurity_ShouldCreateIssuerIfSuchDoesNotExist()
        {
            // Arrange
            var issuerName = "БФБ";
            var ISIN = "BG1100008983";
            var bfbCode = "HSOF";
            var currencyCode = "USD";

            // Act
            await this.DataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);

            // Assert
            Assert.Contains(this.Db.Issuers, i => i.Name == issuerName);
        }

        [Fact]
        public async Task CreateSecurity_ShouldCreateCurrencyIfSuchDoesNotExist()
        {
            // Arrange
            var issuerName = "БФБ";
            var ISIN = "BG1100008983";
            var bfbCode = "HSOF";
            var currencyCode = "USD";

            // Act
            await this.DataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);

            // Assert
            Assert.Contains(this.Db.Currencies, i => i.Code == currencyCode);
        }

        [Fact]
        public void CreateSecurity_ShouldNotCreateSrcurityIfISINOrBFBCodeNotCorrect()
        {
            // Arrange
            var issuerName = "БФБ";
            var ISIN = "BG11000089830";
            var bfbCode = "HSOF5";
            var currencyCode = "USD";
            var expected = false;

            // Act
            var actual = this.DataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode).GetAwaiter().GetResult();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
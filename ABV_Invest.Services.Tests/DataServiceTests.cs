namespace ABV_Invest.Services.Tests
{
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class DataServiceTests
    {
        private readonly AbvDbContext db;
        private readonly IDataService dataService;

        public DataServiceTests()
        {
            var options = new DbContextOptionsBuilder<AbvDbContext>().UseInMemoryDatabase("ABV")
                .Options;
            this.db = new AbvDbContext(options);

            this.dataService = new DataService(this.db);
        }

        [Fact]
        public async Task CreateCurrency_ShouldCreateCurrency()
        {
            // Arrange
            var currencyCode = "USD";
            var expectedCurrenciesCount = 1;

            // Act
            await this.dataService.CreateCurrency(currencyCode);
            var actualCurrenciesCount = this.db.Currencies.Count();

            // Assert
            Assert.Equal(expectedCurrenciesCount, actualCurrenciesCount);
            Assert.Contains(this.db.Currencies, c => c.Code == currencyCode);
        }

        [Fact]
        public async Task CreateCurrency_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            var currencyCode = "USD";
            await this.dataService.CreateCurrency(currencyCode);
            var expected = false;

            // Act
            var actual = await this.dataService.CreateCurrency(currencyCode);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateCurrency_ShouldNotCreateCurrencyIfCodeNotCorrect()
        {
            // Arrange
            var currencyCode = "US1";
            var expected = false;

            // Act
            var actual = await this.dataService.CreateCurrency(currencyCode);

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
            await this.dataService.CreateMarket(marketName, marketCode);
            var actualMarketsCount = this.db.Markets.Count();

            // Assert
            Assert.Equal(expectedMarketsCount, actualMarketsCount);
            Assert.Contains(this.db.Markets, c => c.MIC == marketCode);
        }

        [Fact]
        public async Task CreateMarket_ShouldNotCreateMarketIfSuchAlreadyExists()
        {
            // Arrange
            var marketName = "БФБ";
            var marketCode = "XBUL";
            await this.dataService.CreateMarket(marketName, marketCode);
            var expected = false;

            // Act
            var actual = await this.dataService.CreateMarket(marketName, marketName);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateMarket_ShouldNotCreateMarketIfCodeNotCorrect()
        {
            // Arrange
            var marketName = "БФБ";
            var marketCode = "XBULL";
            var expected = false;

            // Act
            var actual = await this.dataService.CreateMarket(marketName, marketCode);

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
            await this.dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);
            var actualSecuritiesCount = this.db.Securities.Count();

            // Assert
            Assert.Equal(expectedSecuritiesCount, actualSecuritiesCount);
            Assert.Contains(this.db.Securities, c => c.ISIN == ISIN);
            Assert.Contains(this.db.Securities, c => c.BfbCode == bfbCode);
        }

        [Fact]
        public async Task CreateSecurity_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            var issuerName = "БФБ";
            var ISIN = "BG1100008983";
            var bfbCode = "HSOF";
            var currencyCode = "USD";
            await this.dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);
            var expected = false;

            // Act
            var actual = await this.dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);

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
            await this.dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);

            // Assert
            Assert.Contains(this.db.Issuers, i => i.Name == issuerName);
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
            await this.dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);

            // Assert
            Assert.Contains(this.db.Currencies, i => i.Code == currencyCode);
        }

        [Fact]
        public async Task CreateSecurity_ShouldNotCreateSrcurityIfISINOrBFBCodeNotCorrect()
        {
            // Arrange
            var issuerName = "БФБ";
            var ISIN = "BG11000089830";
            var bfbCode = "HSOF5";
            var currencyCode = "USD";
            var expected = false;

            // Act
            var actual = await this.dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
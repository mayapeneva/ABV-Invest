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
            db = new AbvDbContext(options);

            dataService = new DataService(db);
        }

        [Fact]
        public async Task CreateCurrency_ShouldCreateCurrency()
        {
            // Arrange
            var currencyCode = "USD";
            var expectedCurrenciesCount = 1;

            // Act
            await dataService.CreateCurrency(currencyCode);
            var actualCurrenciesCount = db.Currencies.Count();

            // Assert
            Assert.Equal(expectedCurrenciesCount, actualCurrenciesCount);
            Assert.Contains(db.Currencies, c => c.Code == currencyCode);
        }

        [Fact]
        public async Task CreateCurrency_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            var currencyCode = "USD";
            await dataService.CreateCurrency(currencyCode);
            var expected = false;

            // Act
            var actual = await dataService.CreateCurrency(currencyCode);

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
            var actual = await dataService.CreateCurrency(currencyCode);

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
            await dataService.CreateMarket(marketName, marketCode);
            var actualMarketsCount = db.Markets.Count();

            // Assert
            Assert.Equal(expectedMarketsCount, actualMarketsCount);
            Assert.Contains(db.Markets, c => c.MIC == marketCode);
        }

        [Fact]
        public async Task CreateMarket_ShouldNotCreateMarketIfSuchAlreadyExists()
        {
            // Arrange
            var marketName = "БФБ";
            var marketCode = "XBUL";
            await dataService.CreateMarket(marketName, marketCode);
            var expected = false;

            // Act
            var actual = await dataService.CreateMarket(marketName, marketName);

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
            var actual = await dataService.CreateMarket(marketName, marketCode);

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
            await dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);
            var actualSecuritiesCount = db.Securities.Count();

            // Assert
            Assert.Equal(expectedSecuritiesCount, actualSecuritiesCount);
            Assert.Contains(db.Securities, c => c.ISIN == ISIN);
            Assert.Contains(db.Securities, c => c.BfbCode == bfbCode);
        }

        [Fact]
        public async Task CreateSecurity_ShouldNotCreateCurrencyIfSuchAlreadyExists()
        {
            // Arrange
            var issuerName = "БФБ";
            var ISIN = "BG1100008983";
            var bfbCode = "HSOF";
            var currencyCode = "USD";
            await dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);
            var expected = false;

            // Act
            var actual = await dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);

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
            await dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);

            // Assert
            Assert.Contains(db.Issuers, i => i.Name == issuerName);
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
            await dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);

            // Assert
            Assert.Contains(db.Currencies, i => i.Code == currencyCode);
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
            var actual = await dataService.CreateSecurity(issuerName, ISIN, bfbCode, currencyCode);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
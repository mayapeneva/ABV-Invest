namespace ABV_Invest.Services
{
    using Base;
    using Common;
    using Contracts;
    using Data;
    using Models;

    using System.Linq;
    using System.Threading.Tasks;

    public class DataService : BaseService, IDataService

    {
        public DataService(AbvDbContext db)
            : base(db)
        {
        }

        public async Task<bool> CreateCurrency(string code)
        {
            if (this.Db.Currencies.Any(c => c.Code == code))
            {
                return false;
            }

            var currency = new Currency
            {
                Code = code
            };
            if (!DataValidator.IsValid(currency))
            {
                return false;
            }

            await this.Db.Currencies.AddAsync(currency);
            await this.Db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateMarket(string name, string mic)
        {
            if (this.Db.Markets.Any(m => m.Name == name || m.MIC == mic))
            {
                return false;
            }

            var market = new Market
            {
                Name = name,
                MIC = mic
            };
            if (!DataValidator.IsValid(market))
            {
                return false;
            }

            await this.Db.Markets.AddAsync(market);
            await this.Db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateSecurity(string issuerName, string ISIN, string bfbCode, string currencyCode)
        {
            if (this.Db.Securities.Any(s => s.ISIN == ISIN))
            {
                return false;
            }

            var issuer = this.Db.Issuers.SingleOrDefault(i => i.Name == issuerName) ?? await this.CreateIssuer(issuerName);

            var currency = this.Db.Currencies.SingleOrDefault(c => c.Code == currencyCode);
            if (currency is null)
            {
                var result = await this.CreateCurrency(currencyCode);
                if (!result)
                {
                    return false;
                }

                currency = this.Db.Currencies.SingleOrDefault(c => c.Code == currencyCode);
            }

            var security = new Security
            {
                IssuerId = issuer.Id,
                ISIN = ISIN,
                BfbCode = bfbCode,
                Currency = currency
            };

            security.SetSecuritiesType();
            if (!DataValidator.IsValid(security))
            {
                return false;
            }

            await this.Db.Securities.AddAsync(security);
            await this.Db.SaveChangesAsync();

            return true;
        }

        private async Task<Issuer> CreateIssuer(string issuerName)
        {
            var issuer = new Issuer
            {
                Name = issuerName
            };
            if (!DataValidator.IsValid(issuer))
            {
                return null;
            }

            await this.Db.Issuers.AddAsync(issuer);
            await this.Db.SaveChangesAsync();

            return issuer;
        }
    }
}
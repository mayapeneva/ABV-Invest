namespace ABV_Invest.Services.Contracts
{
    using System.Threading.Tasks;

    public interface IDataService
    {
        Task<bool> CreateCurrency(string code);

        Task<bool> CreateMarket(string name, string mic);

        Task<bool> CreateSecurity(string issuerName, string ISIN, string bfbCode, string currencyCode);
    }
}
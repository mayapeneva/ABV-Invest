namespace ABV_Invest.Services.Contracts
{
    using System.Threading.Tasks;
    using BindingModels.Data;

    public interface IDataService
    {
        Task<bool> CreateCurrency(string code);

        Task<bool> CreateMarket(string name);

        Task<bool> CreateSecurity(string issuerName, string ISIN, string bfbCode, string currencyCode);
    }
}
namespace ABV_Invest.Services.Contracts
{
    using BindingModels.Uploads.Portfolios;

    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPortfoliosService
    {
        T[] GetUserDailyPortfolio<T>(ClaimsPrincipal user, DateTime date);

        Task<StringBuilder> SeedPortfolios(IEnumerable<PortfolioRowBindingModel> deserializedPortfolios, DateTime date);
    }
}
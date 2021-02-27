namespace ABV_Invest.Web.Extensions.Contracts
{
    using ABV_Invest.Common.BindingModels.Uploads.Deals;
    using ABV_Invest.Common.BindingModels.Uploads.Portfolios;
    using System.Threading.Tasks;

    public interface IUploadsHelper
    {
        DealRowBindingModel[] DeserialiseDealsUploadedData(string fileName);

        Task SaveUploadedFile(Microsoft.AspNetCore.Http.IFormFile xmlFile, string fileName);

        PortfolioRowBindingModel[] DeserialiseTheUploadedFileData(string fileName);
    }
}
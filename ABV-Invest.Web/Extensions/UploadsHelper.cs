namespace ABV_Invest.Web.Extensions
{
    using ABV_Invest.Common;
    using ABV_Invest.Common.BindingModels.Uploads.Deals;
    using ABV_Invest.Common.BindingModels.Uploads.Portfolios;
    using ABV_Invest.Web.Extensions.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    internal class UploadsHelper : IUploadsHelper
    {
        public DealRowBindingModel[] DeserialiseDealsUploadedData(string fileName)
        {
            var xmlFileContent = File.ReadAllText(fileName);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute(Constants.XmlRootAttr));
            var deserializedDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));
            return deserializedDeals;
        }

        public async Task SaveUploadedFile(Microsoft.AspNetCore.Http.IFormFile xmlFile, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                await xmlFile.CopyToAsync(stream);
            }
        }

        public PortfolioRowBindingModel[] DeserialiseTheUploadedFileData(string fileName)
        {
            var xmlFileContent = System.IO.File.ReadAllText(fileName);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute(Constants.XmlRootAttr));
            var deserializedPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));
            return deserializedPortfolios;
        }
    }
}
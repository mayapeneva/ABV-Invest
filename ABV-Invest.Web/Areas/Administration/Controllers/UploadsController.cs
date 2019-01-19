namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    using BindingModels;
    using BindingModels.Uploads.Deals;
    using BindingModels.Uploads.Portfolios;
    using Common;
    using Services.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Extensions;

    [Area(Constants.Administration)]
    [Authorize(Roles = Constants.Admin)]
    public class UploadsController : Controller
    {
        private const string XmlFileExt = "xml";
        private const string FilePath = "/files/Upload.";

        private readonly IPortfoliosService portfolioService;
        private readonly IDealsService dealsService;
        private readonly IHostingEnvironment environment;

        public UploadsController(IPortfoliosService portfolioService, IHostingEnvironment environment, IDealsService dealsService)
        {
            this.portfolioService = portfolioService;
            this.environment = environment;
            this.dealsService = dealsService;
        }

        public IActionResult PortfoliosInfo()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> PortfoliosInfo(FilesUploadedBindingModel model)
        {
            // Initial data validation
            if (!this.ModelState.IsValid ||
                !DateValidator.ValidateDate(model.Date))
            {
                this.ViewData[Constants.Error] = string.Format(Messages.WrongDate, DateTime.UtcNow.ToString(Constants.DateTimeShortParseFormat));
                return this.View();
            }

            // Processing the XML file
            var xmlFile = model.XMLFile;
            if (xmlFile.ContentType.EndsWith(XmlFileExt))
            {
                var fileName = this.environment.WebRootPath + FilePath + XmlFileExt;
                if (xmlFile.Length > 0)
                {
                    // Saving the uploaded file
                    await this.SaveUploadedFile(xmlFile, fileName);

                    // Deserialising the uploaded file data
                    var deserializedPortfolios = this.DeserialiseTheUploadedFileData(fileName);

                    // Validating the deserialised data
                    if (!DataValidator.IsValid(deserializedPortfolios))
                    {
                        this.ViewData[Constants.Error] = Messages.CouldNotUploadInformation;
                        return this.View();
                    }

                    // Seeding the data from the deserialised file
                    var result = this.portfolioService.SeedPortfolios(deserializedPortfolios, model.Date);
                    this.ViewData[Constants.Error] = result.Result;

                    return this.View();
                }
            }

            // Unsuccessful upload
            this.ViewData[Constants.Error] = Messages.CouldNotUploadInformation;
            return this.View();
        }

        public IActionResult DealsInfo()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> DealsInfo(FilesUploadedBindingModel model)
        {
            // Initial data validation
            if (!this.ModelState.IsValid ||
                !DateValidator.ValidateDate(model.Date))
            {
                this.ViewData[Constants.Error] = string.Format(Messages.WrongDate, DateTime.UtcNow.ToString(Constants.DateTimeShortParseFormat));
                return this.View();
            }

            // Processing the XML file
            var xmlFile = model.XMLFile;
            if (xmlFile.ContentType.EndsWith(XmlFileExt))
            {
                var fileName = this.environment.WebRootPath + FilePath + XmlFileExt;
                if (xmlFile.Length > 0)
                {
                    // Saving the uploaded file
                    await this.SaveUploadedFile(xmlFile, fileName);

                    // Deserialising the uploaded file data
                    DealRowBindingModel[] deserializedDeals = DeserialiseDealsUploadedData(fileName);

                    // Validating the deserialised data
                    if (!DataValidator.IsValid(deserializedDeals))
                    {
                        this.ViewData[Constants.Error] = Messages.CouldNotUploadInformation;
                        return this.View();
                    }

                    // Seeding the data from the deserialised file
                    var result = this.dealsService.SeedDeals(deserializedDeals, model.Date);
                    this.ViewData[Constants.Error] = result.Result;

                    return this.View();
                }
            }

            // Unsuccessful upload
            this.ViewData[Constants.Error] = Messages.CouldNotUploadInformation;
            return this.View();
        }

        private static DealRowBindingModel[] DeserialiseDealsUploadedData(string fileName)
        {
            var xmlFileContent = System.IO.File.ReadAllText(fileName);
            var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute(Constants.XmlRootAttr));
            var deserializedDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));
            return deserializedDeals;
        }

        private async Task SaveUploadedFile(Microsoft.AspNetCore.Http.IFormFile xmlFile, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                await xmlFile.CopyToAsync(stream);
            }
        }

        private PortfolioRowBindingModel[] DeserialiseTheUploadedFileData(string fileName)
        {
            var xmlFileContent = System.IO.File.ReadAllText(fileName);
            var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute(Constants.XmlRootAttr));
            var deserializedPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));
            return deserializedPortfolios;
        }
    }
}
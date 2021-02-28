namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    using ABV_Invest.Common.BindingModels;
    using ABV_Invest.Common.BindingModels.Uploads.Deals;
    using ABV_Invest.Common.Validators;
    using ABV_Invest.Web.Extensions.Contracts;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using System;
    using System.Threading.Tasks;

    [Area(Constants.Administration)]
    [Authorize(Roles = Constants.Admin)]
    public class UploadsController : Controller
    {
        private const string XmlFileExt = "xml";
        private const string FilePath = "/files/Upload.";

        private readonly IPortfoliosService portfolioService;
        private readonly IDealsService dealsService;
        private readonly IUploadsHelper helper;
        private readonly IWebHostEnvironment environment;

        public UploadsController(
            IPortfoliosService portfolioService,
            IDealsService dealsService,
            IUploadsHelper helper,
            IWebHostEnvironment environment)
        {
            this.portfolioService = portfolioService;
            this.dealsService = dealsService;
            this.helper = helper;
            this.environment = environment;
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
                this.ViewData[Constants.Error] = string.Format(Messages.WrongDate, DateTime.UtcNow.ToString(Constants.DateTimeParseFormat));
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
                    await helper.SaveUploadedFile(xmlFile, fileName);

                    // Deserialising the uploaded file data
                    var deserializedPortfolios = helper.DeserialiseTheUploadedFileData(fileName);

                    // Validating the deserialised data
                    if (!DataValidator.IsValid(deserializedPortfolios))
                    {
                        this.ViewData[Constants.Error] = Messages.CouldNotUploadInformation;
                        return this.View();
                    }

                    // Seeding the data from the deserialised file
                    var result = await this.portfolioService.SeedPortfolios(deserializedPortfolios, model.Date);
                    this.ViewData[Constants.Error] = result;
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
                this.ViewData[Constants.Error] = string.Format(Messages.WrongDate, DateTime.UtcNow.ToString(Constants.DateTimeParseFormat));
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
                    await helper.SaveUploadedFile(xmlFile, fileName);

                    // Deserialising the uploaded file data
                    DealRowBindingModel[] deserializedDeals = helper.DeserialiseDealsUploadedData(fileName);

                    // Validating the deserialised data
                    if (!DataValidator.IsValid(deserializedDeals))
                    {
                        this.ViewData[Constants.Error] = Messages.CouldNotUploadInformation;
                        return this.View();
                    }

                    // Seeding the data from the deserialised file
                    var result = await this.dealsService.SeedDeals(deserializedDeals, model.Date);
                    this.ViewData[Constants.Error] = result;
                    return this.View();
                }
            }

            // Unsuccessful upload
            this.ViewData[Constants.Error] = Messages.CouldNotUploadInformation;
            return this.View();
        }
    }
}
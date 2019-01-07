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

    [Area(Constants.Administration)]
    [Authorize(Roles = Constants.Admin)]
    public class UploadsController : Controller
    {
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
            if (!this.ModelState.IsValid
                || model.Date > DateTime.UtcNow
                || model.Date < DateTime.Parse("01/01/2016"))
            {
                this.ViewData["Error"] = string.Format(Messages.WrongDate, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View();
            }

            // Processing the XML file
            var xmlFile = model.XMLFile;
            if (xmlFile.ContentType.EndsWith("xml"))
            {
                var fileName = this.environment.WebRootPath + "/files/" + "Upload.xml";
                if (xmlFile.Length > 0)
                {
                    // Saving the uploaded file
                    using (var stream = new FileStream(fileName, FileMode.Create))
                    {
                        await xmlFile.CopyToAsync(stream);
                    }

                    // Deserialising the uploaded file data
                    var xmlFileContent = System.IO.File.ReadAllText(fileName);
                    var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
                    var deserializedPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

                    // Validating the deserialised data
                    if (!DataValidator.IsValid(deserializedPortfolios))
                    {
                        this.ViewData["Error"] = Messages.CouldNotUploadInformation;
                        return this.View();
                    }

                    // Seeding the data from the deserialised file
                    var result = this.portfolioService.SeedPortfolios(deserializedPortfolios, model.Date);
                    this.ViewData["Error"] = result.Result;

                    return this.View();
                }
            }

            // Unsuccessful upload
            this.ViewData["Error"] = Messages.CouldNotUploadInformation;
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
            if (!this.ModelState.IsValid
                || model.Date > DateTime.UtcNow
                || model.Date < DateTime.Parse("01/01/2016"))
            {
                this.ViewData["Error"] = string.Format(Messages.WrongDate, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View();
            }

            // Processing the XML file
            var xmlFile = model.XMLFile;
            if (xmlFile.ContentType.EndsWith("xml"))
            {
                var fileName = this.environment.WebRootPath + "/files/" + "Upload.xml";
                if (xmlFile.Length > 0)
                {
                    // Saving the uploaded file
                    using (var stream = new FileStream(fileName, FileMode.Create))
                    {
                        await xmlFile.CopyToAsync(stream);
                    }

                    // Deserialising the uploaded file data
                    var xmlFileContent = System.IO.File.ReadAllText(fileName);
                    var serializer = new XmlSerializer(typeof(DealRowBindingModel[]), new XmlRootAttribute("WebData"));
                    var deserializedDeals = (DealRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

                    // Validating the deserialised data
                    if (!DataValidator.IsValid(deserializedDeals))
                    {
                        this.ViewData["Error"] = Messages.CouldNotUploadInformation;
                        return this.View();
                    }

                    // Seeding the data from the deserialised file
                    var result = this.dealsService.SeedDeals(deserializedDeals, model.Date);
                    this.ViewData["Error"] = result.Result;

                    return this.View();
                }
            }

            // Unsuccessful upload
            this.ViewData["Error"] = Messages.CouldNotUploadInformation;
            return this.View();
        }
    }
}
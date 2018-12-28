using Microsoft.AspNetCore.Mvc;

namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using BindingModels;
    using BindingModels.Uploads.Portfolios;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Services.Contracts;

    [Area(Constants.Administration)]
    [Authorize(Roles = Constants.Admin)]
    public class UploadsController : Controller
    {
        private readonly IPortfoliosService portfolioService;
        private readonly IHostingEnvironment environment;

        public UploadsController(IPortfoliosService portfolioService, IHostingEnvironment environment)
        {
            this.portfolioService = portfolioService;
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
                    if (!result.Result)
                    {
                        this.ViewData["Error"] = Messages.CouldNotUploadInformation;
                        return this.View();
                    }

                    // Successful upload
                    this.ViewData["Error"] = Messages.UploadingSuccessfull;
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
            return this.View();
        }
    }
}
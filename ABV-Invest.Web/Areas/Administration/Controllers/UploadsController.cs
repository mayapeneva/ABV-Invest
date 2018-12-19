using Microsoft.AspNetCore.Mvc;

namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using BindingModels;
    using BindingModels.Uploads.Portfolios;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Services.Contracts;

    [Area("Administration")]
    [Authorize(Roles = "Admin")]
    public class UploadsController : Controller
    {
        private readonly IPortfoliosService portfolioService;

        public UploadsController(IPortfoliosService portfolioService)
        {
            this.portfolioService = portfolioService;
        }

        public IActionResult PortfoliosInfo()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> PortfoliosInfo(FilesUploadedBindingModel model)
        {
            // Validation
            if (!this.ModelState.IsValid
                || model.Date > DateTime.UtcNow
                || model.Date < DateTime.Parse("01/01/2016"))
            {
                this.ViewBag.Error = string.Format(Messages.WrongDate, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View();
            }

            // Saving the uploaded file and deserialing the XML file
            var xmlFile = model.XMLFile;
            if (xmlFile.ContentType.EndsWith(".xml"))
            {
                var filePath = Path.GetTempPath();
                if (xmlFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await xmlFile.CopyToAsync(stream);
                    }

                    var xmlFileContent = System.IO.File.ReadAllText(filePath + xmlFile.FileName);
                    var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
                    var objPortfolios = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

                    var result = this.portfolioService.SeedPortfolios(objPortfolios.ToList(), model.Date);
                    if (!result.IsCompleted)
                    {
                        this.ViewBag.Error = Messages.CouldNotUploadInformation;
                        return this.View();
                    }
                }
            }

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
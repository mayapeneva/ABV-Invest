using Microsoft.AspNetCore.Mvc;

namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;
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
            if (xmlFile.ContentType.EndsWith("xml"))
            {
                var filePath = Path.GetFullPath("XMLFile");
                if (xmlFile.Length > 0)
                {
                    //Unfortunatelly this way did not work :(
                    //using (var stream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    await xmlFile.CopyToAsync(stream);
                    //}

                    //var xmlFileContent = System.IO.File.ReadAllText(filePath);
                    //var serializer = new XmlSerializer(typeof(PortfolioRowBindingModel[]), new XmlRootAttribute("WebData"));
                    //var deserializedPortfolioss = (PortfolioRowBindingModel[])serializer.Deserialize(new StringReader(xmlFileContent));

                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);

                    XmlNodeList portfoliosNodeList = xmlDoc.SelectNodes("/WebData/New");
                    var deserializedPortfolios = new List<PortfolioRowBindingModel>();
                    foreach (XmlNode portfolioNode in portfoliosNodeList)
                    {
                        var portfolio = new PortfolioRowBindingModel
                        {
                            Client = new Client(),
                            Instrument = new Instrument(),
                            AccountData = new AccountData(),
                            Other = new Other()
                        };
                        foreach (XmlElement element in portfolioNode)
                        {
                            if (element.Name == "Client")
                            {
                                portfolio.Client.CDNNumber = element.SelectSingleNode("CDNumber").InnerText;
                                portfolio.Client.Name = element.SelectSingleNode("Name").InnerText;
                            }

                            if (element.Name == "Instrument")
                            {
                                portfolio.Instrument.ISIN = element.SelectSingleNode("ISIN").InnerText;
                                portfolio.Instrument.NewCode = element.SelectSingleNode("NewCode").InnerText;
                                portfolio.Instrument.Issuer = element.SelectSingleNode("Issuer").InnerText;
                                portfolio.Instrument.Currency = element.SelectSingleNode("Currency").InnerText;

                                var date = element.SelectSingleNode("MaturityDate").InnerText;
                                var ifParsed = DateTime.TryParse(date,
                                    out DateTime maturityDate);
                                if (!ifParsed)
                                {
                                    maturityDate = default(DateTime);
                                }
                                portfolio.Instrument.MaturityDate = maturityDate;
                            }

                            if (element.Name == "AccountData")
                            {
                                var number = element.SelectSingleNode("Quantity").InnerText.Replace(".00000).00000.0000", "").Replace(" ", "");
                                portfolio.AccountData.Quantity = decimal.Parse(number);

                                number = element.SelectSingleNode("OpenPrice").InnerText.Replace(".00000).00000.0000", "").Replace(" ", "");
                                portfolio.AccountData.OpenPrice = decimal.Parse(number);

                                number = element.SelectSingleNode("MarketPrice").InnerText.Replace(".00000).00000.0000", "").Replace(" ", "");
                                portfolio.AccountData.MarketPrice = decimal.Parse(number);

                                number = element.SelectSingleNode("MarketValue").InnerText.Replace(".00000).00000.0000", "").Replace(" ", "");
                                portfolio.AccountData.MarketValue = decimal.Parse(number);

                                var date = element.SelectSingleNode("MarketDate").InnerText;
                                var ifParsed = DateTime.TryParse(date,
                                    out DateTime marketDate);
                                if (!ifParsed)
                                {
                                    marketDate = default(DateTime);
                                }
                                portfolio.AccountData.MarketDate = marketDate;

                                number = element.SelectSingleNode("Result").InnerText.Replace(".00000).00000.0000", "").Replace(" ", "");
                                portfolio.AccountData.Result = decimal.Parse(number);

                                number = element.SelectSingleNode("ResultBGN").InnerText.Replace(".00000).00000.0000", "").Replace(" ", "");
                                portfolio.AccountData.ResultBGN = decimal.Parse(number);
                            }

                            if (element.Name == "Other")
                            {
                                var number = element.SelectSingleNode("YieldPercent").InnerText.Replace(".00000).00000.0000", "").Replace(" ", "");
                                portfolio.Other.YieldPercent = decimal.Parse(number);

                                number = element.SelectSingleNode("RelativePart").InnerText.Replace(".00000).00000.0000", "").Replace(" ", "");
                                portfolio.Other.RelativePart = decimal.Parse(number);
                            }
                        }

                        deserializedPortfolios.Add(portfolio);
                    }

                    var result = this.portfolioService.SeedPortfolios(deserializedPortfolios, model.Date);
                    if (!result.IsCompleted)
                    {
                        this.ViewBag.Error = Messages.CouldNotUploadInformation;
                        return this.View();
                    }
                }
            }

            this.ViewBag.Error = Messages.UploadingSuccessfull;
            return this.View();
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
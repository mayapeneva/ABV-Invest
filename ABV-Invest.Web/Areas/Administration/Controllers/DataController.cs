namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    using BindingModels.Data;
    using Common;
    using Services.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area(Constants.Administration)]
    [Authorize(Roles = Constants.Admin)]
    public class DataController : Controller
    {
        private readonly IDataService dataService;

        public DataController(IDataService dataService)
        {
            this.dataService = dataService;
        }

        public IActionResult Add()
        {
            return this.View();
        }

        public IActionResult AddCurrency()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult AddCurrency(CurrencyBindingModel bindingModel)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewData[Constants.Error] = Messages.WrongCurrencyData;
                return this.View();
            }

            var result = this.dataService.CreateCurrency(bindingModel.Code);
            if (!result.Result)
            {
                this.ViewData[Constants.Error] = Messages.CurrencyExists;
                return this.View();
            }

            return this.RedirectToAction(Constants.AddAction);
        }

        public IActionResult AddMarket()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult AddMarket(MarketBindingModel bindingModel)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewData[Constants.Error] = Messages.WrongMarketData;
                return this.View();
            }

            var result = this.dataService.CreateMarket(bindingModel.Name, bindingModel.MIC);
            if (!result.Result)
            {
                this.ViewData[Constants.Error] = Messages.MarketExists;
                return this.View();
            }

            return this.RedirectToAction(Constants.AddAction);
        }

        public IActionResult AddSecurity()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult AddSecurity(SecurityBindingModel bindingModel)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewData[Constants.Error] = Messages.WrongSecurityData;
                return this.View();
            }

            var result = this.dataService.CreateSecurity(bindingModel.Issuer, bindingModel.ISIN, bindingModel.BfbCode, bindingModel.Currency);
            if (!result.Result)
            {
                this.ViewData[Constants.Error] = Messages.SecurityExists;
                return this.View();
            }

            return this.RedirectToAction(Constants.AddAction);
        }
    }
}
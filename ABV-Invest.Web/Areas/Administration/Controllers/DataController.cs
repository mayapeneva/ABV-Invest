namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    using BindingModels.Data;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;

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
                this.ViewData["Error"] = Messages.WrongCurrencyData;
                return this.View();
            }

            var result = this.dataService.CreateCurrency(bindingModel.Code);
            if (!result.Result)
            {
                this.ViewData["Error"] = Messages.CurrencyExists;
                return this.View();
            }

            return this.RedirectToAction("Add");
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
                this.ViewData["Error"] = Messages.WrongMarketData;
                return this.View();
            }

            var result = this.dataService.CreateMarket(bindingModel.Name);
            if (!result.Result)
            {
                this.ViewData["Error"] = Messages.MarketExists;
                return this.View();
            }

            return this.RedirectToAction("Add");
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
                this.ViewData["Error"] = Messages.WrongSecurityData;
                return this.View();
            }

            var result = this.dataService.CreateSecurity(bindingModel.Issuer, bindingModel.ISIN, bindingModel.BfbCode, bindingModel.Currency);
            if (!result.Result)
            {
                this.ViewData["Error"] = Messages.SecurityExists;
                return this.View();
            }

            return this.RedirectToAction("Add");
        }
    }
}
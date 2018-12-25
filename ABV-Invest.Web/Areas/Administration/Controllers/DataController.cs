namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    using BindingModels.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;

    [Area("Administration")]
    [Authorize(Roles = "Admin")]
    public class DataController : Controller
    {
        private const string WrongCurrencyData = "Валутният код се състои от 3 главни латински букви.";
        private const string CurrencyExists = "Валута с този код вече съществува.";
        private const string WrongMarketData = "Името на пазара е неправилно.";
        private const string MarketExists = "Пазар с това име вече съществува.";

        private const string WrongSecurityData =
            "ISIN кодът се състои от 12 цифри или главни латински букви. БФБ кодът се състои от 3 или 4 цифри или главни латински букви.";

        private const string SecurityExists = "Ценни книжа с този ISIN код вече съществуват.";

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
                this.ViewData["Error"] = WrongCurrencyData;
                return this.View();
            }

            var result = this.dataService.CreateCurrency(bindingModel.Code);
            if (!result.Result)
            {
                this.ViewData["Error"] = CurrencyExists;
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
                this.ViewData["Error"] = WrongMarketData;
                return this.View();
            }

            var result = this.dataService.CreateMarket(bindingModel.Name);
            if (!result.Result)
            {
                this.ViewData["Error"] = MarketExists;
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
                this.ViewData["Error"] = WrongSecurityData;
                return this.View();
            }

            var result = this.dataService.CreateSecurity(bindingModel.Issuer, bindingModel.ISIN, bindingModel.BfbCode, bindingModel.Currency);
            if (!result.Result)
            {
                this.ViewData["Error"] = SecurityExists;
                return this.View();
            }

            return this.RedirectToAction("Add");
        }
    }
}
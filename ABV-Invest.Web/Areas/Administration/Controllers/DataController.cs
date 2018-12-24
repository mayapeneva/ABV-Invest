using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    using BindingModels.Data;
    using Microsoft.AspNetCore.Authorization;

    [Area("Administration")]
    [Authorize(Roles = "Admin")]
    public class DataController : Controller
    {
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
            return View();
        }

        public IActionResult AddMarket()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult AddMarket(MarketBindingModel bindingModel)
        {
            return View();
        }

        public IActionResult AddSecurity()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult AddSecurity(SecurityBindingModel bindingModel)
        {
            return View();
        }
    }
}
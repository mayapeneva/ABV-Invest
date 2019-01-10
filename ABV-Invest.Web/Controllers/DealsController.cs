namespace ABV_Invest.Web.Controllers
{
    using AutoMapper;
    using BindingModels;
    using Common;
    using DTOs;
    using Services.Contracts;
    using ViewModels;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;

    [Authorize]
    public class DealsController : Controller
    {
        private readonly IDealsService dealsService;

        public DealsController(IDealsService dealsService)
        {
            this.dealsService = dealsService;
        }

        public IActionResult ChooseDate()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult ChooseDate(DateChosenBindingModel dateChosen)
        {
            if (!this.ModelState.IsValid
                || dateChosen.Date > DateTime.UtcNow
                || dateChosen.Date < DateTime.Parse("01/01/2016"))
            {
                this.ViewData["Error"] = string.Format(Messages.NoDeals, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View();
            }

            this.TempData["Date"] = dateChosen.Date;

            return this.RedirectToAction("Details");
        }

        public IActionResult Details()
        {
            var date = (DateTime)this.TempData["Date"];
            var deals = this.dealsService.GetUserDailyDeals<DealDto>(this.User, date);

            if (deals == null)
            {
                this.ViewData["Error"] = string.Format(Messages.NoDeals, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View("ChooseDate");
            }

            var dealsViewModel = Mapper.Map<DealDto[], IEnumerable<DealViewModel>>(deals);

            return this.View(dealsViewModel);
        }
    }
}
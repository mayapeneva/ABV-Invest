namespace ABV_Invest.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using ABV_Invest.Models;
    using AutoMapper;
    using BindingModels;
    using Common;
    using DTOs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using ViewModels;

    [Authorize]
    public class DealsController : Controller
    {
        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IDealsService dealsService;

        public DealsController(UserManager<AbvInvestUser> userManager, IDealsService dealsService)
        {
            this.userManager = userManager;
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

            return this.RedirectToAction("Details", new { date = dateChosen.Date.ToString("dd/MM/yyyy") });
        }

        public IActionResult Details(string date)
        {
            var user = this.userManager.GetUserAsync(this.User).GetAwaiter().GetResult();
            var deals = this.dealsService.GetUserDailyDeals<DealDto>(user, date);

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
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

    public class DealsController : Controller
    {
        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IDealsService dealsService;
        private readonly IMapper mapper;

        public DealsController(UserManager<AbvInvestUser> userManager, IDealsService dealsService, IMapper mapper)
        {
            this.userManager = userManager;
            this.dealsService = dealsService;
            this.mapper = mapper;
        }

        public IActionResult ChooseDate()
        {
            return this.View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChooseDate(DateChosenBindingModel dateChosen)
        {
            if (!this.ModelState.IsValid
                || dateChosen.Date > DateTime.UtcNow
                || dateChosen.Date < DateTime.Parse("01/01/2016"))
            {
                this.ViewBag.ErrorMessage = Messages.NoDeals;
                return this.View();
            }

            this.TempData["DateChosen"] = dateChosen.Date;

            return this.RedirectToAction("Details", new { date = dateChosen.Date.ToString("dd/MM/yyyy") });
        }

        public IActionResult Details(string date)
        {
            var userId = this.userManager.GetUserId(this.User);
            var deals = this.dealsService.GetUserDailyDeals(userId, date);

            if (deals == null)
            {
                this.ViewBag.ErrorMessage = Messages.NoDeals;
                return this.View("ChooseDate");
            }

            var dealsViewModel = this.mapper.Map<DealsDto[], IEnumerable<DealsViewModel>>(deals);

            return this.View(dealsViewModel);
        }
    }
}
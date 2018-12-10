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
    using Models;
    using Services.Contracts;
    using ViewModels;

    public class PortfoliosController : Controller
    {
        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IPortfoliosService portfoliosService;

        public PortfoliosController(UserManager<AbvInvestUser> userManager, IPortfoliosService portfoliosService)
        {
            this.userManager = userManager;
            this.portfoliosService = portfoliosService;
        }

        [Authorize]
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
                this.ViewBag.ErrorMessage = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View();
            }

            this.TempData["DateChosen"] = dateChosen.Date;

            return this.RedirectToAction("Details", new { date = dateChosen.Date.ToString("dd/MM/yyyy") });
        }

        [Authorize]
        public IActionResult Details(string date)
        {
            var userId = this.userManager.GetUserId(this.User);
            var portfolio = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(userId, date);

            if (portfolio == null)
            {
                this.ViewBag.ErrorMessage = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View("ChooseDate");
            }

            var portfolioViewModel = Mapper.Map<PortfolioDto[], IEnumerable<PortfolioViewModel>>(portfolio);

            return this.View(portfolioViewModel);
        }
    }
}
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
    public class PortfoliosController : Controller
    {
        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IPortfoliosService portfoliosService;

        public PortfoliosController(UserManager<AbvInvestUser> userManager, IPortfoliosService portfoliosService)
        {
            this.userManager = userManager;
            this.portfoliosService = portfoliosService;
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
                this.ViewData["Error"] = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View();
            }

            return this.RedirectToAction("Details", new { date = dateChosen.Date.ToString("dd/MM/yyyy") });
        }

        public IActionResult Details(string date)
        {
            var userId = this.userManager.GetUserId(this.User);
            var portfolio = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(userId, date);

            if (portfolio == null)
            {
                this.ViewData["Error"] = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View("ChooseDate");
            }

            var portfolioViewModel = Mapper.Map<PortfolioDto[], IEnumerable<PortfolioViewModel>>(portfolio);

            return this.View(portfolioViewModel);
        }
    }
}
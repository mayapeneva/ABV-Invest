namespace ABV_Invest.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using ABV_Invest.Models;
    using AutoMapper;
    using BindingModels;
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
        private readonly IMapper mapper;

        public PortfoliosController(UserManager<AbvInvestUser> userManager, IPortfoliosService portfoliosService, IMapper mapper)
        {
            this.userManager = userManager;
            this.portfoliosService = portfoliosService;
            this.mapper = mapper;
        }

        public IActionResult Index()
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
                return this.View();
            }

            this.TempData["DateChosen"] = dateChosen.Date;

            return this.RedirectToAction("Details", "Portfolios", new { date = dateChosen.Date.ToString("dd/MM/yyyy") });
        }

        public IActionResult Details(string date)
        {
            var userId = this.userManager.GetUserId(this.User);
            var portfolio = this.portfoliosService.GetUserPortfolio(userId, date);
            if (portfolio == null)
            {
                return this.View();
            }

            var portfolioViewModel = this.mapper.Map<PortfolioDto[], IEnumerable<PortfolioViewModel>>(portfolio);

            return this.View(portfolioViewModel);
        }
    }
}
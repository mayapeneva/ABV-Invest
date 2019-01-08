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
    using Rotativa.AspNetCore;
    using System;
    using System.Collections.Generic;
    using Rotativa.AspNetCore.Options;

    [Authorize]
    public class PortfoliosController : Controller
    {
        private readonly IPortfoliosService portfoliosService;

        public PortfoliosController(IPortfoliosService portfoliosService)
        {
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
            var portfolio = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.User, date);
            if (portfolio == null)
            {
                this.ViewData["Error"] = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View("ChooseDate");
            }

            var portfolioViewModel = Mapper.Map<PortfolioDto[], IEnumerable<PortfolioViewModel>>(portfolio);

            return this.View(portfolioViewModel);
        }

        [HttpPost]
        public IActionResult CreatePdf(string date)
        {
            var portfolio = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.User, date);
            if (portfolio == null)
            {
                this.ViewData["Error"] = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View("ChooseDate");
            }

            var portfolioViewModel = Mapper.Map<PortfolioDto[], IEnumerable<PortfolioViewModel>>(portfolio);

            return new ViewAsPdf("CreatePdf", portfolioViewModel)
            {
                PageOrientation = Orientation.Landscape,
                PageSize = Size.A4,
                PageMargins = { Left = 15, Bottom = 10, Right = 15, Top = 10 },
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 6"
            };
        }
    }
}
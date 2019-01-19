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
    using Extensions;
    using Rotativa.AspNetCore.Options;

    [Authorize]
    public class PortfoliosController : Controller
    {
        private const string CreatePDF = "CreatePdf";
        private const string CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 6";

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
            if (!this.ModelState.IsValid ||
                !DateValidator.ValidateDate(dateChosen.Date))
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString(Constants.DateTimeShortParseFormat));
                return this.View();
            }

            this.TempData[Constants.Date] = dateChosen.Date;

            return this.RedirectToAction(Constants.DetailsAction);
        }

        public IActionResult Details()
        {
            var date = (DateTime)this.TempData[Constants.Date];
            var portfolio = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.User, date);
            if (portfolio == null)
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString(Constants.DateTimeShortParseFormat));
                return this.View(Constants.ChooseDateAction);
            }

            var portfolioViewModel = Mapper.Map<PortfolioDto[], IEnumerable<PortfolioViewModel>>(portfolio);

            return this.View(portfolioViewModel);
        }

        [HttpPost]
        public IActionResult CreatePdf(string date)
        {
            var parsedDate = DateTime.Parse(date);
            var portfolio = this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.User, parsedDate);
            if (portfolio == null)
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString(Constants.DateTimeShortParseFormat));
                return this.View(Constants.ChooseDateAction);
            }

            var portfolioViewModel = Mapper.Map<PortfolioDto[], IEnumerable<PortfolioViewModel>>(portfolio);

            return new ViewAsPdf(CreatePDF, portfolioViewModel)
            {
                PageOrientation = Orientation.Landscape,
                PageSize = Size.A4,
                PageMargins = { Left = 15, Bottom = 10, Right = 15, Top = 10 },
                CustomSwitches = CustomSwitches
            };
        }
    }
}
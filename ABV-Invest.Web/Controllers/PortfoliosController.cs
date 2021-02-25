namespace ABV_Invest.Web.Controllers
{
    using ABV_Invest.Common.BindingModels;
    using ABV_Invest.Common.DTOs;
    using AutoMapper;
    using Common;
    using Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Rotativa.AspNetCore;
    using Rotativa.AspNetCore.Options;
    using Services.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ViewModels;

    [Authorize]
    public class PortfoliosController : Controller
    {
        private const string Portfolio = "Portfolio";
        private const string PdfExt = ".pdf";
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
                this.ViewData[Constants.Error] = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString(Constants.DateTimeParseFormat));
                return this.View();
            }

            this.TempData[Constants.Date] = dateChosen.Date;
            return this.RedirectToAction(Constants.DetailsAction);
        }

        public async Task<IActionResult> Details()
        {
            var date = (DateTime)this.TempData[Constants.Date];
            var portfolio = await this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.User, date);
            if (portfolio is null)
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString(Constants.DateTimeParseFormat));
                return this.View(Constants.ChooseDateAction);
            }

            var portfolioViewModel = Mapper.Map<PortfolioDto[], IEnumerable<PortfolioViewModel>>(portfolio);
            return this.View(portfolioViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePdf(string date)
        {
            var parsedDate = DateTime.Parse(date);
            var portfolio = await this.portfoliosService.GetUserDailyPortfolio<PortfolioDto>(this.User, parsedDate);
            if (portfolio is null)
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoPortfolio, DateTime.UtcNow.ToString(Constants.DateTimeParseFormat));
                return this.View(Constants.ChooseDateAction);
            }

            var portfolioViewModel = Mapper.Map<PortfolioDto[], IEnumerable<PortfolioViewModel>>(portfolio);
            return new ViewAsPdf(CreatePDF, portfolioViewModel)
            {
                FileName = Portfolio + "_" + date + PdfExt,
                PageOrientation = Orientation.Landscape,
                PageSize = Size.A4,
                PageMargins = { Left = 15, Bottom = 10, Right = 15, Top = 10 },
                CustomSwitches = CustomSwitches
            };
        }
    }
}
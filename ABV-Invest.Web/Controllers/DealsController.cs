namespace ABV_Invest.Web.Controllers
{
    using ABV_Invest.Common.BindingModels;
    using ABV_Invest.Common.DTOs;
    using ABV_Invest.Common.Validators;
    using AutoMapper;
    using Common;
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
    public class DealsController : Controller
    {
        private const string Deals = "Deals";
        private const string PdfExt = ".pdf";
        private const string CreatePDF = "CreatePdf";
        private const string CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 6";

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
            if (!this.ModelState.IsValid ||
                !DateValidator.ValidateDate(dateChosen.Date))
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoDeals, DateTime.UtcNow.ToString(Constants.DateTimeParseFormat));
                return this.View();
            }

            this.TempData[Constants.Date] = dateChosen.Date;
            return this.RedirectToAction(Constants.DetailsAction);
        }

        public async Task<IActionResult> Details()
        {
            var date = (DateTime)this.TempData[Constants.Date];
            var deals = await this.dealsService.GetUserDailyDeals<DealDto>(this.User, date);
            if (deals is null)
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoDeals, DateTime.UtcNow.ToString(Constants.DateTimeParseFormat));
                return this.View(Constants.ChooseDateAction);
            }

            var dealsViewModel = Mapper.Map<DealDto[], IEnumerable<DealViewModel>>(deals);
            return this.View(dealsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePdf(string date)
        {
            var parsedDate = DateTime.Parse(date);
            var deals = await this.dealsService.GetUserDailyDeals<DealDto>(this.User, parsedDate);
            if (deals is null)
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoDeals, DateTime.UtcNow.ToString(Constants.DateTimeParseFormat));
                return this.View(Constants.ChooseDateAction);
            }

            var dealsViewModel = Mapper.Map<DealDto[], IEnumerable<DealViewModel>>(deals);
            return new ViewAsPdf(CreatePDF, dealsViewModel)
            {
                FileName = Deals + "_" + date + PdfExt,
                PageOrientation = Orientation.Landscape,
                PageSize = Size.A4,
                PageMargins = { Left = 15, Bottom = 10, Right = 15, Top = 10 },
                CustomSwitches = CustomSwitches
            };
        }
    }
}
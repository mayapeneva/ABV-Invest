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
    using Extensions;

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
            if (!this.ModelState.IsValid ||
                !DateValidator.ValidateDate(dateChosen.Date))
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoDeals, DateTime.UtcNow.ToString(Constants.DateTimeShortParseFormat));
                return this.View();
            }

            this.TempData[Constants.Date] = dateChosen.Date;

            return this.RedirectToAction(Constants.DetailsAction);
        }

        public IActionResult Details()
        {
            var date = (DateTime)this.TempData[Constants.Date];
            var deals = this.dealsService.GetUserDailyDeals<DealDto>(this.User, date);

            if (deals == null)
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoDeals, DateTime.UtcNow.ToString(Constants.DateTimeShortParseFormat));
                return this.View(Constants.ChooseDateAction);
            }

            var dealsViewModel = Mapper.Map<DealDto[], IEnumerable<DealViewModel>>(deals);

            return this.View(dealsViewModel);
        }
    }
}
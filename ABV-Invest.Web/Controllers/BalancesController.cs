namespace ABV_Invest.Web.Controllers
{
    using ABV_Invest.Models;
    using AutoMapper;
    using BindingModels;
    using Common;
    using DTOs;
    using Services.Contracts;
    using ViewModels;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using Extensions;

    [Authorize]
    public class BalancesController : Controller
    {
        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IBalancesService balancesService;

        public BalancesController(UserManager<AbvInvestUser> userManager, IBalancesService balancesService)
        {
            this.userManager = userManager;
            this.balancesService = balancesService;
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
                this.ViewData[Constants.Error] = string.Format(Messages.NoBalance, DateTime.UtcNow.ToString(Constants.DateTimeShortParseFormat));
                return this.View();
            }

            this.TempData[Constants.Date] = dateChosen.Date;

            return this.RedirectToAction(Constants.DetailsAction);
        }

        public IActionResult Details()
        {
            var date = (DateTime)this.TempData[Constants.Date];
            var user = this.userManager.GetUserAsync(this.User).GetAwaiter().GetResult();
            var balance = this.balancesService.GetUserDailyBalance<BalanceDto>(user, date);

            if (balance == null)
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoBalance, DateTime.UtcNow.ToString(Constants.DateTimeShortParseFormat));
                return this.View(Constants.ChooseDateAction);
            }

            var balanceViewModel = Mapper.Map<BalanceViewModel>(balance);

            return this.View(balanceViewModel);
        }
    }
}
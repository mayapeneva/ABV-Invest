namespace ABV_Invest.Web.Controllers
{
    using ABV_Invest.Common.BindingModels;
    using ABV_Invest.Common.DTOs;
    using ABV_Invest.Common.Validators;
    using ABV_Invest.Models;
    using AutoMapper;
    using Common;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using System;
    using System.Threading.Tasks;
    using ViewModels;

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
                this.ViewData[Constants.Error] = string.Format(Messages.NoBalance, DateTime.UtcNow.ToString(Constants.DateTimeParseFormat));
                return this.View();
            }

            this.TempData[Constants.Date] = dateChosen.Date;
            return this.RedirectToAction(Constants.DetailsAction);
        }

        public async Task<IActionResult> Details()
        {
            var date = (DateTime)this.TempData[Constants.Date];
            var user = await this.userManager.GetUserAsync(this.User);
            var balance = this.balancesService.GetUserDailyBalance<BalanceDto>(user, date);
            if (balance is null)
            {
                this.ViewData[Constants.Error] = string.Format(Messages.NoBalance, DateTime.UtcNow.ToString(Constants.DateTimeParseFormat));
                return this.View(Constants.ChooseDateAction);
            }

            var balanceViewModel = Mapper.Map<BalanceViewModel>(balance);
            return this.View(balanceViewModel);
        }
    }
}
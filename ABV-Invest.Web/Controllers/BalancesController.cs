namespace ABV_Invest.Web.Controllers
{
    using System;
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
            if (!this.ModelState.IsValid
                || dateChosen.Date > DateTime.UtcNow
                || dateChosen.Date < DateTime.Parse("01/01/2016"))
            {
                this.ViewBag.Error = string.Format(Messages.NoBalance, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View();
            }

            this.TempData["DateChosen"] = dateChosen.Date;

            return this.RedirectToAction("Details", new { date = dateChosen.Date.ToString("dd/MM/yyyy") });
        }

        public IActionResult Details(string date)
        {
            var userId = this.userManager.GetUserId(this.User);
            var balance = this.balancesService.GetUserDailyBalance<BalanceDto>(userId, date);

            if (balance == null)
            {
                this.ViewBag.Error = string.Format(Messages.NoBalance, DateTime.UtcNow.ToString("dd/MM/yyyy"));
                return this.View("ChooseDate");
            }

            var balanceViewModel = Mapper.Map<BalanceViewModel>(balance);

            return this.View(balanceViewModel);
        }
    }
}
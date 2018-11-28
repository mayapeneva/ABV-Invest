namespace ABV_Invest.Web.Controllers
{
    using System;
    using ABV_Invest.Models;
    using BindingModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using ViewModels;

    public class PortfoliosController : Controller
    {
        private readonly UserManager<AbvInvestUser> userManager;
        private readonly IPortfoliosService portfoliosService;

        public PortfoliosController(UserManager<AbvInvestUser> userManager, IPortfoliosService portfoliosService)
        {
            this.userManager = userManager;
            this.portfoliosService = portfoliosService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(DateChosenBindingModel dateChosen)
        {
            if (!this.ModelState.IsValid
                || dateChosen.Date > DateTime.UtcNow
                || dateChosen.Date < DateTime.Parse("01/01/2016"))
            {
                return this.View();
            }

            this.TempData["DateChosen"] = dateChosen.Date;

            return this.RedirectToAction("Details");
        }

        public IActionResult Details()
        {
            var userId = this.userManager.GetUserId(this.User);
            var portfolio = this.portfoliosService.GetUserPortfolio(userId, this.TempData["DateChosen"].ToString());

            return this.View(portfolio);
        }
    }
}
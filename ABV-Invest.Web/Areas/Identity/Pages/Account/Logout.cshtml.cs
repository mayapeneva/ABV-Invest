using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ABV_Invest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ABV_Invest.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AbvInvestUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<AbvInvestUser> signInManager, ILogger<LogoutModel> logger)
        {
            this._signInManager = signInManager;
            this._logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await this._signInManager.SignOutAsync();
            this._logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return this.LocalRedirect(returnUrl);
            }
            else
            {
                return this.Page();
            }
        }
    }
}
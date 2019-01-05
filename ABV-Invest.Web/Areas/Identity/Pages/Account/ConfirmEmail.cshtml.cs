namespace ABV_Invest.Web.Areas.Identity.Pages.Account
{
    using ABV_Invest.Models;
    using Common;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<AbvInvestUser> _userManager;

        public ConfirmEmailModel(UserManager<AbvInvestUser> userManager)
        {
            this._userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return this.RedirectToPage("/Index");
            }

            var user = await this._userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return this.NotFound(string.Format(Messages.CantLoadUser, userId));
            }

            var result = await this._userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Възникна грешка при потвърждаването на имейл за потребител с ID '{userId}':");
            }

            return this.Page();
        }
    }
}
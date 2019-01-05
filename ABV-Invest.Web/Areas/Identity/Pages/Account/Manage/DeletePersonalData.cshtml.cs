namespace ABV_Invest.Web.Areas.Identity.Pages.Account.Manage
{
    using ABV_Invest.Models;
    using Common;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<AbvInvestUser> _userManager;
        private readonly SignInManager<AbvInvestUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;

        public DeletePersonalDataModel(
            UserManager<AbvInvestUser> userManager,
            SignInManager<AbvInvestUser> signInManager,
            ILogger<DeletePersonalDataModel> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound(string.Format(Messages.CantLoadUser, this._userManager.GetUserId(this.User)));
            }

            this.RequirePassword = await this._userManager.HasPasswordAsync(user);
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound(string.Format(Messages.CantLoadUser, this._userManager.GetUserId(this.User)));
            }

            this.RequirePassword = await this._userManager.HasPasswordAsync(user);
            if (this.RequirePassword)
            {
                if (!await this._userManager.CheckPasswordAsync(user, this.Input.Password))
                {
                    this.ModelState.AddModelError(string.Empty, "Неправилна парола.");
                    return this.Page();
                }
            }

            var result = await this._userManager.DeleteAsync(user);
            var userId = await this._userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Format(Messages.MistakeWhenDeleting, userId));
            }

            await this._signInManager.SignOutAsync();

            this._logger.LogInformation("Потребител с ID '{UserId}' изтри регистрацията си.", userId);

            return this.Redirect("~/");
        }
    }
}
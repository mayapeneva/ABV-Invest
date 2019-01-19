namespace ABV_Invest.Web.Areas.Identity.Pages.Account.Manage
{
    using ABV_Invest.Models;
    using Common;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<AbvInvestUser> _userManager;
        private readonly SignInManager<AbvInvestUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<AbvInvestUser> userManager,
            SignInManager<AbvInvestUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = Messages.PasswordLengthError, MinimumLength = 6)]
            [DataType(DataType.Password, ErrorMessage = Messages.PasswordTypeError)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = Messages.PasswordsDontMatch)]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound(string.Format(Messages.CantLoadUser, this._userManager.GetUserId(this.User)));
            }

            var hasPassword = await this._userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return this.RedirectToPage(Constants.SetPassword);
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound(string.Format(Messages.CantLoadUser, this._userManager.GetUserId(this.User)));
            }

            var changePasswordResult = await this._userManager.ChangePasswordAsync(user, this.Input.OldPassword, this.Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
                return this.Page();
            }

            await this._signInManager.RefreshSignInAsync(user);
            this._logger.LogInformation(Messages.PasswordChangeSuccess);
            this.StatusMessage = Messages.YourPasswordChangedSuccess;

            return this.RedirectToPage();
        }
    }
}
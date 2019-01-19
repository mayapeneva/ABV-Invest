namespace ABV_Invest.Web.Areas.Identity.Pages.Account.Manage
{
    using ABV_Invest.Models;
    using Common;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<AbvInvestUser> _userManager;
        private readonly SignInManager<AbvInvestUser> _signInManager;

        public SetPasswordModel(
            UserManager<AbvInvestUser> userManager,
            SignInManager<AbvInvestUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = Messages.PasswordLengthError, MinimumLength = 6)]
            [DataType(DataType.Password, ErrorMessage = Messages.PasswordTypeError)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("Password", ErrorMessage = Messages.PasswordsDontMatch)]
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

            if (hasPassword)
            {
                return this.RedirectToPage(Constants.ChangePassword);
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

            var addPasswordResult = await this._userManager.AddPasswordAsync(user, this.Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
                return this.Page();
            }

            await this._signInManager.RefreshSignInAsync(user);
            this.StatusMessage = Messages.PasswordAccepted;

            return this.RedirectToPage();
        }
    }
}
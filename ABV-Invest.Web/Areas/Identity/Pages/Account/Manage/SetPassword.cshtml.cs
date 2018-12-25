using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ABV_Invest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ABV_Invest.Web.Areas.Identity.Pages.Account.Manage
{
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
            [StringLength(100, ErrorMessage = "Паролата трябва да е дълга поне {2} и не повече от {1} символа.", MinimumLength = 6)]
            [DataType(DataType.Password, ErrorMessage = "Паролата трябва да съдържа поне по една малка, една голяма буква, цифра и символ.")]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("Password", ErrorMessage = "Двете въведени пароли не са еднакви.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
            }

            var hasPassword = await this._userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return this.RedirectToPage("./ChangePassword");
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
                return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
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
            this.StatusMessage = "Your password has been set.";

            return this.RedirectToPage();
        }
    }
}
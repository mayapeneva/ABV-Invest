namespace ABV_Invest.Web.Areas.Identity.Pages.Account
{
    using ABV_Invest.Models;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Common;

    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<AbvInvestUser> _userManager;

        public ResetPasswordModel(UserManager<AbvInvestUser> userManager)
        {
            this._userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.EmailAddress, ErrorMessage = Messages.ValidEmail)]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = Messages.PasswordLengthError, MinimumLength = 6)]
            [DataType(DataType.Password, ErrorMessage = Messages.PasswordTypeError)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = Messages.PasswordsDontMatch)]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return this.BadRequest(Messages.CodeNeededForPasswordChange);
            }
            else
            {
                this.Input = new InputModel
                {
                    Code = code
                };
                return this.Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = await this._userManager.FindByEmailAsync(this.Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return this.RedirectToPage(Constants.ResetPasswordConfirmation);
            }

            var result = await this._userManager.ResetPasswordAsync(user, this.Input.Code, this.Input.Password);
            if (result.Succeeded)
            {
                return this.RedirectToPage(Constants.ResetPasswordConfirmation);
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
            return this.Page();
        }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ABV_Invest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ABV_Invest.Web.Areas.Identity.Pages.Account
{
    using Common;
    using Data;
    using DTOs;
    using Services.Contracts;

    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AbvInvestUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IUsersService usersService;

        public LoginModel(SignInManager<AbvInvestUser> signInManager, ILogger<LoginModel> logger, AbvDbContext db, IUsersService usersService)
        {
            this._signInManager = signInManager;
            this._logger = logger;
            this.usersService = usersService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [RegularExpression(@"^[A-Z0-9]{5}$|^[A-Z0-9]{10}$", ErrorMessage = "Потребителското име трябва да е дълго 5 или 10 символа и да съдържа цифри и/или главни латински букви.")]
            public string UserName { get; set; }

            [Required]
            [RegularExpression(@"^\d{5}$", ErrorMessage = "ПИН кодът трябва да е дълъг 5 символа и да съдържа само цифри.")]
            [Display(Name = "PIN")]
            public string PIN { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(this.ErrorMessage))
            {
                this.ModelState.AddModelError(string.Empty, this.ErrorMessage);
            }

            returnUrl = returnUrl ?? this.Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ExternalLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await this._signInManager.PasswordSignInAsync(this.Input.UserName, this.Input.Password, this.Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var dbUser = this.usersService.GetUserByUserName<UserDto>(this.Input.UserName);
                    if (dbUser == null || this.Input.PIN != dbUser.PIN)
                    {
                        this.ModelState.AddModelError(string.Empty, Messages.InvalidLogInAttempt);
                        return this.Page();
                    }

                    this._logger.LogInformation(Messages.UserLoggedIn);
                    return this.LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return this.RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = this.Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    this._logger.LogWarning(Messages.LockedAccount);
                    return this.RedirectToPage("./Lockout");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, Messages.InvalidLogInAttempt);
                    return this.Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}
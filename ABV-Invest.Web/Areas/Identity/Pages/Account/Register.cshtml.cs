namespace ABV_Invest.Web.Areas.Identity.Pages.Account
{
    using ABV_Invest.Models;
    using Common;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AbvInvestUser> _signInManager;
        private readonly UserManager<AbvInvestUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<AbvInvestUser> userManager,
            SignInManager<AbvInvestUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
            this._emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [RegularExpression(Constants.UserNameRegex, ErrorMessage = Messages.UsernameError)]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [RegularExpression(Constants.PINRegex, ErrorMessage = Messages.PINError)]
            [Display(Name = "PIN")]
            public string PIN { get; set; }

            [DataType(DataType.EmailAddress, ErrorMessage = Messages.EmailError)]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = Messages.PasswordLengthError, MinimumLength = 6)]
            [DataType(DataType.Password, ErrorMessage = Messages.PasswordTypeError)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = Messages.PasswordsDontMatch)]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGet(string returnUrl = null)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                await this.LocalRedirect("/").ExecuteResultAsync(this.PageContext);
            }

            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");
            if (this.ModelState.IsValid)
            {
                var dbUser = this._userManager.Users.SingleOrDefault(u => u.UserName == this.Input.Username);
                if (dbUser != null)
                {
                    this.ViewData[Constants.Error] = Messages.UserExists;
                    return this.Page();
                }

                var user = new AbvInvestUser { UserName = this.Input.Username, PIN = this.Input.PIN, Email = this.Input.Email };
                var result = await this._userManager.CreateAsync(user, this.Input.Password);
                if (result.Succeeded)
                {
                    await this._userManager.AddToRoleAsync(user, Constants.User);

                    this._logger.LogInformation(Messages.NewRegistrationWithPassword);

                    var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = this.Url.Page(
                        Constants.ConfirmEmail,
                        null,
                        new { userId = user.Id, code },
                        this.Request.Scheme);

                    if (!string.IsNullOrWhiteSpace(this.Input.Email))
                    {
                        await this._emailSender.SendEmailAsync(this.Input.Email, Messages.ConfirmEmail, string.Format(Messages.RegistrationConfirmation, HtmlEncoder.Default.Encode(callbackUrl)));
                    }

                    await this._signInManager.SignInAsync(user, isPersistent: false);
                    return this.LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}
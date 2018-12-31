using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ABV_Invest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ABV_Invest.Web.Areas.Identity.Pages.Account
{
    using System.Linq;
    using Common;
    using DTOs;
    using Services.Contracts;

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
            [RegularExpression(@"^[A-Z0-9]{5}$|^[A-Z0-9]{10}$", ErrorMessage = "Потребителското име трябва да е дълго 5 или 10 символа и да съдържа цифри и/или главни латински букви.")]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [RegularExpression(@"^\d{5}$", ErrorMessage = "ПИН кодът трябва да е дълъг 5 символа и да съдържа само цифри.")]
            [Display(Name = "PIN")]
            public string PIN { get; set; }

            [DataType(DataType.EmailAddress, ErrorMessage = "Моля въведете валиден имейл адрес.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Паролата трябва да е дълга поне {2} и не повече от {1} символа.", MinimumLength = 6)]
            [DataType(DataType.Password, ErrorMessage = "Паролата трябва да съдържа поне по една малка, една голяма буква, цифра и символ.")]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Двете въведени пароли не са еднакви.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
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
                    return this.Page();
                }

                var user = new AbvInvestUser { UserName = this.Input.Username, PIN = this.Input.PIN, Email = this.Input.Email };
                var result = await this._userManager.CreateAsync(user, this.Input.Password);
                if (result.Succeeded)
                {
                    await this._userManager.AddToRoleAsync(user, Constants.User);

                    this._logger.LogInformation("Потребителят създаде нова регистрация с парола.");

                    var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: this.Request.Scheme);

                    await this._emailSender.SendEmailAsync(this.Input.Email, "Потвърдете имейла си.",
                        $"Моля потвърдете регистрацията си като кликнете <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>тук</a>.");

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
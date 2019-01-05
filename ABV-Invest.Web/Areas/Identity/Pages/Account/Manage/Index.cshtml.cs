namespace ABV_Invest.Web.Areas.Identity.Pages.Account.Manage
{
    using ABV_Invest.Models;
    using Common;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AbvInvestUser> _userManager;
        private readonly SignInManager<AbvInvestUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            UserManager<AbvInvestUser> userManager,
            SignInManager<AbvInvestUser> signInManager,
            IEmailSender emailSender)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailSender = emailSender;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound(string.Format(Messages.CantLoadUser, this._userManager.GetUserId(this.User)));
            }

            var userName = await this._userManager.GetUserNameAsync(user);
            var email = await this._userManager.GetEmailAsync(user);
            var phoneNumber = await this._userManager.GetPhoneNumberAsync(user);

            this.Username = userName;

            this.Input = new InputModel
            {
                Email = email,
                PhoneNumber = phoneNumber
            };

            this.IsEmailConfirmed = await this._userManager.IsEmailConfirmedAsync(user);

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

            var email = await this._userManager.GetEmailAsync(user);
            if (this.Input.Email != email)
            {
                var setEmailResult = await this._userManager.SetEmailAsync(user, this.Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await this._userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException(string.Format(Messages.MistakeWhenSaving, "имейл", userId));
                }
            }

            var phoneNumber = await this._userManager.GetPhoneNumberAsync(user);
            if (this.Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await this._userManager.SetPhoneNumberAsync(user, this.Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await this._userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException(string.Format(Messages.MistakeWhenSaving, "телефонен номер", userId));
                }
            }

            await this._signInManager.RefreshSignInAsync(user);
            this.StatusMessage = Messages.ProfileChanged;
            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
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

            var userId = await this._userManager.GetUserIdAsync(user);
            var email = await this._userManager.GetEmailAsync(user);
            var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = this.Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: this.Request.Scheme);
            await this._emailSender.SendEmailAsync(
                email,
                "Потвърдете имейла си",
                $"Моля потвърдете регистрацията си като клинкнете <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>тук</a>.");

            this.StatusMessage = "Изпратихме Ви имейл за потвърждение. Моля проверете имейла си.";
            return this.RedirectToPage();
        }
    }
}
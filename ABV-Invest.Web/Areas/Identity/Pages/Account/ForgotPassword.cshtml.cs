namespace ABV_Invest.Web.Areas.Identity.Pages.Account
{
    using System;
    using ABV_Invest.Models;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Common;

    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<AbvInvestUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<AbvInvestUser> userManager, IEmailSender emailSender)
        {
            this._userManager = userManager;
            this._emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.EmailAddress, ErrorMessage = Messages.ValidEmail)]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (this.ModelState.IsValid)
            {
                var user = await this._userManager.FindByEmailAsync(this.Input.Email);
                if (user == null || !(await this._userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return this.RedirectToPage(Constants.ForgotPassword);
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await this._userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = this.Url.Page(
                    Constants.ResetPassword,
                    pageHandler: null,
                    values: new { code },
                    protocol: this.Request.Scheme);

                await this._emailSender.SendEmailAsync(this.Input.Email,
                    Constants.PasswordChange, String.Format(Messages.ChangePassword, HtmlEncoder.Default.Encode(callbackUrl)));

                return this.RedirectToPage(Constants.ForgotPassword);
            }

            return this.Page();
        }
    }
}
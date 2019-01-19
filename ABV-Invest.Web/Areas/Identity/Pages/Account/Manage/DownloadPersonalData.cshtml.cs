namespace ABV_Invest.Web.Areas.Identity.Pages.Account.Manage
{
    using ABV_Invest.Models;
    using Common;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DownloadPersonalDataModel : PageModel
    {
        private const string ContentDisposition = "Content-Disposition";
        private const string ContentDispositionValue = "attachment; filename=PersonalData.json";

        private readonly UserManager<AbvInvestUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;

        public DownloadPersonalDataModel(
            UserManager<AbvInvestUser> userManager,
            ILogger<DownloadPersonalDataModel> logger)
        {
            this._userManager = userManager;
            this._logger = logger;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound(string.Format(Messages.CantLoadUser, this._userManager.GetUserId(this.User)));
            }

            this._logger.LogInformation(string.Format(Messages.PersonalDataRequested, user.Id), this._userManager.GetUserId(this.User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(AbvInvestUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            this.Response.Headers.Add(ContentDisposition, ContentDispositionValue);
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), Constants.TextJson);
        }
    }
}
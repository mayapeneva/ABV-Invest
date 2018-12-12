using Microsoft.AspNetCore.Mvc;

namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;

    [Authorize(Roles = "Admin")]
    public class UploadsController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
namespace ABV_Invest.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class BalancesController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
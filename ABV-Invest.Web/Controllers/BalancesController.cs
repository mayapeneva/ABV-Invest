namespace ABV_Invest.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class BalancesController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
namespace ABV_Invest.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class BalancesController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
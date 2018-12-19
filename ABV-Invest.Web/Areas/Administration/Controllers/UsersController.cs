using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ABV_Invest.Web.Areas.Administration.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Maintenance()
        {
            return View();
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ABV_Invest.Web.Controllers
{
    public class PortfoliosController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
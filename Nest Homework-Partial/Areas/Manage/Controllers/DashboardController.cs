using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_Homework_Partial.Areas.Manage.Controllers
{
    [Area("manage")]
    public class DashboardController : Controller
    {       
        public IActionResult Index()
        {
            return View();
        }
    }
}

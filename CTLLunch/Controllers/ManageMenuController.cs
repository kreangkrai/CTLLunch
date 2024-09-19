using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Controllers
{
    public class ManageMenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

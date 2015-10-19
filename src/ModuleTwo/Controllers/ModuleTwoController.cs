using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ModuleTwo.Controllers
{
    public class ModuleTwoController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.ModuleName = "ModuleTwo";

            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace YantraJS.WebSite.Controllers
{
    public class HomeModel
    {
        public string Message { get; set; }
    }

    [Route("Home")]
    public class HomeController : Controller
    {

        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View(new HomeModel { 
                Message = "Demo Message"
            });
        }
    }
}

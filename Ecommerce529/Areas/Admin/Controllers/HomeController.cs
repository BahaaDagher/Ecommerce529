using Microsoft.AspNetCore.Mvc;

namespace Ecommerce529.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

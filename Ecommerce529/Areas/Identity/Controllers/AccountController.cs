using Microsoft.AspNetCore.Mvc;

namespace Ecommerce529.Areas.Identity.Controllers
{
    [Area(CD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}

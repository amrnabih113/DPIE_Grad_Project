using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

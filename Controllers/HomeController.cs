using Microsoft.AspNetCore.Mvc;

namespace TEST1_SCADA.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

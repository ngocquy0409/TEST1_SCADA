using Microsoft.AspNetCore.Mvc;

namespace TEST1_SCADA.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

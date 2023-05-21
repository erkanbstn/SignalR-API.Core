using Microsoft.AspNetCore.Mvc;

namespace SignalRUIWeb.Core.Controllers
{
    public class CovidController : Controller
    {
        public IActionResult Covid19Show()
        {
            return View();
        }
    }
}

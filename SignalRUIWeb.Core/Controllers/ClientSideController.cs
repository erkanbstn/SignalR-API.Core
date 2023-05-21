using Microsoft.AspNetCore.Mvc;

namespace SignalRUIWeb.Core.Controllers
{
    public class ClientSideController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

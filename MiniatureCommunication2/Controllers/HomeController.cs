using Microsoft.AspNetCore.Mvc;

namespace MiniatureCommunication2.Controllers {
	public class HomeController : Controller {
		public HomeController() {

		}

		public IActionResult Index() => View();
	}
}

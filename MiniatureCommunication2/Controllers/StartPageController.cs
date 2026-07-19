using WebLoginDemo.Models.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebLoginDemo.Controllers {
	[Authorize]
	public class StartPageController : Controller {
		private readonly ServerDbContext _db;
		public StartPageController(ServerDbContext db) => _db = db;


		
		public IActionResult Index() {
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> Save(string t1,string t2) {
			_db.Userdata.Add(new UserdataModel { Test1 = t1,Test2=t2 });
			await _db.SaveChangesAsync();
			Console.WriteLine(t1 + ", " + t2);
			return View("Index");
		}
	}
}

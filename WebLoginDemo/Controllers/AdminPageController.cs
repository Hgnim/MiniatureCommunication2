using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using WebLoginDemo.Models.database;
using static WebLoginDemo.DataCore;

namespace WebLoginDemo.Controllers {
	[Authorize(Roles = "Owner")]
	public class AdminController : Controller {

		private readonly ServerDbContext _db;
		public AdminController(ServerDbContext db) {
			_db = db; 
		}

		//[Route("~/admin")]
		public IActionResult Index() {
				return View();
		}


		public async Task<IActionResult> CreateInviteCode() {
			string? code = Convert.ToBase64String(RandomNumberGenerator.GetBytes(9))
							   .Replace("/", "").Replace("+", ""); //替换不安全字符
			_db.InviteCode.Add(new InviteCode {
				Code = code,
				ExpireAt = DateTime.UtcNow.AddDays(7),//7天后过期
				Role = "User"
			});
			await _db.SaveChangesAsync();
			return await GetInviteCode();
		}

		public async Task<IActionResult> GetInviteCode() {			
			InviteCode? code;
			do {//自动清理失效的邀请码
				code = await _db.InviteCode.FirstOrDefaultAsync(c => c.Used == true || (c.ExpireAt != null/*永久有效*/ && c.ExpireAt < DateTime.UtcNow));
				if (code != null) {
					_db.InviteCode.Remove(code);//删除一个已使用或过期的邀请码
					await _db.SaveChangesAsync();
				}
			} while (code != null);
			return Json(string.Join("\n", await _db.InviteCode.Select(c => c.Code).ToListAsync())); 
		}
	}
}

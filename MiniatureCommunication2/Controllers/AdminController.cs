using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniatureCommunication2.Database;
using MiniatureCommunication2.Models.Admin;
using System.Security.Claims;
using System.Security.Cryptography;
using IdentityUser = MiniatureCommunication2.Database.IdentityUser;

namespace MiniatureCommunication2.Controllers {
	[Authorize(Roles = "Owner")]
	public class AdminController : Controller {

		private readonly ServerDbContext _db;
		//private readonly IConfiguration _config;
		private readonly UserManager<IdentityUser> _userManager;
		public AdminController(
			ServerDbContext db,
			//IConfiguration config
			UserManager<IdentityUser> userManager
			) {
			_db = db;
			//_config = config;
			_userManager = userManager;
		}

		//[Route("~/admin")]
		public IActionResult Index() {
			var indexModel = new IndexModel {
				//PathBase = _config["Config:Server:PathBase"]
			};
			return View(indexModel);
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
			return Json(
					await _db.InviteCode.Select(s => new InviteCode {
						Code = s.Code,
						Used = s.Used,
						ExpireAt = s.ExpireAt,
						Role = s.Role,
					}).ToListAsync()
				); 
		}

		public async Task<IActionResult> ClearInviteCode() {
			InviteCode? code;
			do {//自动清理失效的邀请码
				code = await _db.InviteCode.FirstOrDefaultAsync(c => c.Used == true || (c.ExpireAt != null/*永久有效*/ && c.ExpireAt < DateTime.UtcNow));
				if (code != null) {
					_db.InviteCode.Remove(code);//删除一个已使用或过期的邀请码
					await _db.SaveChangesAsync();
				}
			} while (code != null);
			return await GetInviteCode();//return Ok();
		}


		public async Task<IActionResult> GetUserList() {
			return Json(
					await _userManager.Users.Select(s => new IdentityUserMini {
						Id = s.Id,
						UserName = s.UserName,
						LockoutEnd = s.LockoutEnd,
					}).ToListAsync()
				);
		}
		[HttpPost]
		public async Task<IActionResult> LockUser([FromBody] IdentityUserMini ium) {
			var user = await _userManager.FindByIdAsync(ium.Id);//通过Id查找用户
			if (user == null) return NotFound();

			//最大时间锁定，等于永久锁定
			var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

			return result.Succeeded
					? Ok()
					: BadRequest(/*result.Errors*/);
		}

		[HttpPost]
		public async Task<IActionResult> UnlockUser([FromBody] IdentityUserMini ium) {
			var user = await _userManager.FindByIdAsync(ium.Id);//通过Id查找用户
			if (user == null) return NotFound();

			//设置为null以解锁
			var result = await _userManager.SetLockoutEndDateAsync(user, null);

			return result.Succeeded
					? Ok()
					: BadRequest();
		}
		[HttpPost]
		public async Task<IActionResult> DeleteUser([FromBody] IdentityUserMini ium) {//!! 批注：后续将可能有更多用户数据，删除用户时还需要额外删除相关用户数据
			var user = await _userManager.FindByIdAsync(ium.Id);//通过Id查找用户
			if (user == null) return NotFound();

			var result = await _userManager.DeleteAsync(user);

			return result.Succeeded
					? Ok()
					: BadRequest();
		}
	}
}

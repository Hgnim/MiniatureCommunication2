using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniatureCommunication2.Database;

namespace MiniatureCommunication2.Controllers {
	[Authorize(Roles = "Owner,User")]
	public class TalkController : Controller {
		private readonly ServerDbContext _db;
		private readonly UserManager<Database.IdentityUser> _userManager;

		public TalkController(
			ServerDbContext db,
			UserManager<Database.IdentityUser> userManager
			) {
			_db = db;
			_userManager = userManager;
		}

		public IActionResult Index() {
			return View();
		}

		public async Task<IActionResult> GetConversationList() {
			var user = await _userManager.GetUserAsync(User);//获取当前登录的用户数据
			if (user != null) {
				List<long> allCid = [];
				{
					//该用户作为成员的记录
					var member = await _db.ConversationMember
						.Where(p => p.UserId == user.Id).ToListAsync();
					foreach (var m in member)
						allCid.Add(m.ConversationId);
				}

				List<ConversationDTO_CrMember> ccm = [];
				{
					var conversations = await _db.Conversation
						.Where(p => allCid.Contains(p.Id))//通过数组内容是否包含目标id来查询
						.ToListAsync();
					foreach (var c in conversations)
						ccm.Add(new ConversationDTO_CrMember {
							Id = c.Id,
							Type = c.Type,
							Group_Title = c.Group_Title,
							LastMessageId = c.LastMessageId,
						});
				}

				return Json(ccm);
			}
			else return Unauthorized();
		}
	}
}

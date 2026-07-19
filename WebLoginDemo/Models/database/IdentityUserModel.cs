using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebLoginDemo.Models.database {
	public class IdentityUserModel:IdentityUser {
		// 使用纯用户名，Email永远为空
		public override string? Email { get => null; set => _ = value; }
	}

	public class InviteCode {
		[Key]
		public string Code { get; set; }//邀请码（主键）

		public bool Used { get; set; }//是否已使用
		public DateTime? ExpireAt { get; set; }//过期时间
		public string? Role { get; set; }//预置角色
	}
}

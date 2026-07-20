using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MiniatureCommunication2.Database {
	public class IdentityUser: Microsoft.AspNetCore.Identity.IdentityUser {
		// 使用纯用户名，Email永远为空
		public override string? Email { get => null; set => _ = value; }
	}

	/// <summary>
	/// IdentityUser的简化版本，仅包含部分属性
	/// </summary>
	public class IdentityUserMini {
		public required string Id { get; set; }
		public string? UserName { get; set; }
		public DateTimeOffset? LockoutEnd { get; set; }
	}

	public class InviteCode {
		/// <summary>
		/// 邀请码（主键）
		/// </summary>
		[Key]
		public required string Code { get; set; }

		/// <summary>
		/// 是否已使用
		/// </summary>
		public bool Used { get; set; }
		/// <summary>
		/// 过期时间
		/// </summary>
		public DateTime? ExpireAt { get; set; }
		/// <summary>
		/// 预置角色
		/// </summary>
		public string? Role { get; set; }
	}
}

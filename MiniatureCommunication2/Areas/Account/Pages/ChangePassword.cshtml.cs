using WebLoginDemo.Models.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebLoginDemo.Areas.Account.Pages {

    [AllowAnonymous]
    public class ChangePasswordModel : PageModel {

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public class InputModel {
			[Required(ErrorMessage = "用户名不能为空")]
			//[EmailAddress]
			[Display(Name = "用户名")]
			public string UserName { get; set; } = default!;

			[Required(ErrorMessage = "旧密码不能为空")]
			[DataType(DataType.Password)]
			[Display(Name = "旧密码")]
			public string OldPassword { get; set; } = default!;

			[Required(ErrorMessage = "新密码不能为空")]
			[StringLength(100, ErrorMessage = "{0}长度必须至少为 {2} 个字符，最多为 {1} 个字符。", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "新密码")]
			public string Password { get; set; } = default!;

            [DataType(DataType.Password)]
			[Display(Name = "确认新密码")]
			[Compare("Password", ErrorMessage = "密码和确认密码不一致。")]
			public string? ConfirmPassword { get; set; }
        }


		private readonly UserManager<IdentityUserModel> _userManager;
		private readonly SignInManager<IdentityUserModel> _signInManager;

		public ChangePasswordModel(
			UserManager<IdentityUserModel> userManager,
			SignInManager<IdentityUserModel> signInManager) {
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public void OnGet() {
		}


		public async Task<IActionResult> OnPostAsync() {
			if (!ModelState.IsValid)
				return Page();

			var user = await _userManager.FindByNameAsync(Input.UserName);//.FindByEmailAsync(Input.Email);
			Microsoft.AspNetCore.Identity.SignInResult pwCheck;
			{
				void checkFailed() {
					ModelState.AddModelError(string.Empty, "用户名或旧密码错误。");
				}
				if (user == null) {
					checkFailed();
					return Page();
				}
				pwCheck = await _signInManager.CheckPasswordSignInAsync(user, Input.OldPassword, lockoutOnFailure: true/*启用多次错误后锁定账户*/);
				if (!pwCheck.Succeeded) {
					if (pwCheck.IsLockedOut)
						return RedirectToPage("./Lockout");
					checkFailed();
					return Page();
				}
			}

			var result = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.Password);
			if (result.Succeeded) {
				await _userManager.UpdateSecurityStampAsync(user);//更改密码后，更新安全戳，已登录的设备将被迫重新登录
				await _signInManager.RefreshSignInAsync(user);//刷新登录状态
				return RedirectToPage("./ChangePasswordConfirmation");
			}

			foreach (var error in result.Errors) {
				ModelState.AddModelError(string.Empty, error.Description);
			}
			return Page();
		}
    }
}
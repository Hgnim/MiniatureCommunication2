//source: https://github.com/dotnet/aspnetcore/blob/v8.0.19/src/Identity/UI/src/Areas/Identity/Pages/V5/Account/Login.cshtml.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebLoginDemo.Models.database;

namespace WebLoginDemo.Areas.Account.Pages {
	public class LoginModel : PageModel {
		[BindProperty]
		public InputModel Input { get; set; }
		public string ReturnUrl { get; set; }
		[TempData]
		public string ErrorMessage { get; set; }
	
		public class InputModel {
			[Required(ErrorMessage ="用户名不能为空")]
			//[EmailAddress]
			[Display(Name = "用户名")]
			public string UserName { get; set; }= string.Empty;

			[Required(ErrorMessage ="密码不能为空")]
			[DataType(DataType.Password)]
			[Display(Name = "密码")]
			public string Password { get; set; } = string.Empty;

			[Display(Name = "保持登录")]
			public bool RememberMe { get; set; }
		}

		private readonly SignInManager<IdentityUserModel> _signInManager;
		private readonly ILogger<LoginModel> _logger;

		public LoginModel(
			SignInManager<IdentityUserModel> signInManager, 
			ILogger<LoginModel> logger) {
			_signInManager = signInManager;
			_logger = logger;
		}
		public async Task OnGetAsync(string? returnUrl = null) {
			if (!string.IsNullOrEmpty(ErrorMessage)) 				ModelState.AddModelError(string.Empty, ErrorMessage);

			returnUrl ??= Url.Content("~/");

			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync(string? returnUrl = null) {
			returnUrl ??= Url.Content("~/");

			if (ModelState.IsValid) {
				var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: true);
				if (result.Succeeded) {
					_logger.LogInformation("用户已登录。");
					return LocalRedirect(returnUrl);
				}
				if (result.RequiresTwoFactor) 					return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
				if (result.IsLockedOut) {
					_logger.LogWarning("账户已被锁定。");
					return RedirectToPage("./Lockout");
				}
				else {
					ModelState.AddModelError(string.Empty, "无效的登录尝试。");
					return Page();
				}
			}

			return Page();
		}
	}
}
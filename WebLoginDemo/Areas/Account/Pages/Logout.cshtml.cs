// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// source: https://github.com/dotnet/aspnetcore/blob/v8.0.19/src/Identity/UI/src/Areas/Identity/Pages/V5/Account

using WebLoginDemo.Models.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebLoginDemo.Areas.Account.Pages {
    [AllowAnonymous]
    public class LogoutModel : PageModel {

		private readonly SignInManager<IdentityUserModel> _signInManager;
		private readonly ILogger<LogoutModel> _logger;

		public LogoutModel(SignInManager<IdentityUserModel> signInManager, ILogger<LogoutModel> logger) {
			_signInManager = signInManager;
			_logger = logger;
		}

		public void OnGet() {
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null) {
			await _signInManager.SignOutAsync();
			_logger.LogInformation(LoggerEventIds.UserLoggedOut, "用户已登出。");
			if (returnUrl != null)
				return LocalRedirect(returnUrl);
			else {
				// This needs to be a redirect so that the browser performs a new
				// request and the identity for the user gets updated.
				return RedirectToPage();
			}
		}
    }
}

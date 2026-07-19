// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// source: https://github.com/dotnet/aspnetcore/blob/v8.0.19/src/Identity/UI/src/Areas/Identity/Pages/V5/Account

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebLoginDemo.Areas.Account.Pages {
    [AllowAnonymous]
    public class LockoutModel : PageModel {
        public void OnGet() {
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MiniatureCommunication2.Areas.Account.Pages
{
	[AllowAnonymous]
	public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

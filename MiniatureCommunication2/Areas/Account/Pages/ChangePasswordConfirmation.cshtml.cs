using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MiniatureCommunication2.Areas.Account.Pages {
    [AllowAnonymous]
    public class ChangePasswordConfirmationModel : PageModel {
        public void OnGet() {
        }
    }
}

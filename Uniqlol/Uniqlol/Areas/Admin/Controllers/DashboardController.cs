using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uniqlol.Enums;

namespace Uniqlol.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {

        [Area("Admin")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public IActionResult Index()
        {
            return View();
        }
    }
}

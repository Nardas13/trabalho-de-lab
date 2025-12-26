using Microsoft.AspNetCore.Mvc;

namespace AutoHubProjeto.Controllers.Admin
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult EntrarAdmin()
        {
            HttpContext.Session.SetString("AdminMode", "true");
            return RedirectToAction("Index", "AdmDashboard");
        }

        [HttpGet]
        public IActionResult Sair()
        {
            HttpContext.Session.Remove("AdminMode");
            return RedirectToAction("Index", "Home");
        }
    }
}

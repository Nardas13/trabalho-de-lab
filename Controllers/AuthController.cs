using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string Email, string Password)
    {
        // Depois ligamos à BD
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Register()
    {
        // Depois ligamos à BD
        return RedirectToAction("Preferencias");
    }

    public IActionResult Preferencias()
    {
        return View();
    }
}

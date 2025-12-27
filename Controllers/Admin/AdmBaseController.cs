using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AutoHubProjeto.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.Controllers.Admin
{
    public abstract class AdmBaseController : Controller
    {
        protected readonly ApplicationDbContext _db;

        protected AdmBaseController(ApplicationDbContext db)
        {
            _db = db;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Autenticado?
            if (!User.Identity!.IsAuthenticated)
            {
                context.Result = RedirectToAction("Index", "Auth");
                return;
            }

            // Buscar user
            var email = User.Identity.Name;

            var user = _db.Utilizadors
                .Include(u => u.Administrador)
                .FirstOrDefault(u => u.Email == email);

            if (user == null || user.Administrador == null)
            {
                context.Result = RedirectToAction("Index", "Home");
                return;
            }

            // Está em modo ADMIN?
            if (HttpContext.Session.GetString("AdminMode") != "true")
            {
                context.Result = RedirectToAction("Index", "Painel");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}

using AutoHubProjeto.Models;
using AutoHubProjeto.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.ViewComponents
{
    public class MenuPainelViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public MenuPainelViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // se não estiver autenticado
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return View("~/Views/Painel/_PainelMenu.cshtml", new PainelMenuVM());

            var email = User.Identity!.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                .Include(u => u.Vendedor)
                .Include(u => u.Administrador)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return View("~/Views/Painel/_PainelMenu.cshtml", new PainelMenuVM());

            int favoritos = 0;

            // Contar favoritos se for comprador
            if (user.Comprador != null)
            {
                favoritos = await _db.Favoritos
                    .CountAsync(f => f.IdComprador == user.Comprador.IdComprador);
            }

            var vm = new PainelMenuVM
            {
                IsComprador = user.Comprador != null,
                IsVendedor = user.Vendedor != null,
                IsAdmin = user.Administrador != null,

                FavoritosCount = favoritos       
            };

            return View("~/Views/Painel/_PainelMenu.cshtml", vm);
        }
    }
}

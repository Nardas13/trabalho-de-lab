using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _db;

        public VeiculosController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Detalhes(int id)
        {
            var anuncio = _db.Anuncios
                .Include(a => a.IdVeiculoNavigation)
                .Include(a => a.AnuncioImagems)
                .Include(a => a.IdVendedorNavigation)
                .FirstOrDefault(a => a.IdAnuncio == id);

            if (anuncio == null)
                return NotFound();

            // verificar se está favoritado
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;

                var user = _db.Utilizadors
                    .Include(u => u.Comprador)
                    .FirstOrDefault(u => u.Email == email);

                if (user?.Comprador != null)
                {
                    anuncio.IsFavorito = _db.Favoritos
                        .Any(f => f.IdAnuncio == id && f.IdComprador == user.Comprador.IdComprador);
                }
            }

            return View(anuncio);
        }

    }
}

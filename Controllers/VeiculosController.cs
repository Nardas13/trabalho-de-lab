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
            // Carregar anúncio + veículo + imagens + vendedor
            var anuncio = _db.Anuncios
                .Include(a => a.IdVeiculoNavigation)
                .Include(a => a.AnuncioImagems)
                .Include(a => a.IdVendedorNavigation)
                .Include(a => a.Visita)
                .FirstOrDefault(a => a.IdAnuncio == id);

            if (anuncio == null)
                return NotFound();

            // Inicializa ViewBag
            ViewBag.UserVendedorId = null;

            // Se o utilizador estiver autenticado
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;

                var user = _db.Utilizadors
                    .Include(u => u.Comprador)
                    .Include(u => u.Vendedor)
                    .FirstOrDefault(u => u.Email == email);

                // Preencher favoritos
                if (user?.Comprador != null)
                {
                    anuncio.IsFavorito = _db.Favoritos
                        .Any(f => f.IdAnuncio == id && f.IdComprador == user.Comprador.IdComprador);
                }

                // Enviar ID do vendedor autenticado para a View
                if (user?.Vendedor != null)
                {
                    ViewBag.UserVendedorId = user.Vendedor.IdVendedor;
                }
            }

            return View(anuncio);
        }

    }
}

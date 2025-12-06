using AutoHubProjeto.Models;
using AutoHubProjeto.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.Controllers
{
    public class PainelController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PainelController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.Identity.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                .Include(u => u.Vendedor)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return RedirectToAction("Login", "Auth");

            int idComprador = user.Comprador?.IdComprador ?? 0;
            int idVendedor = user.Vendedor?.IdVendedor ?? 0;

            var vm = new PainelDashboardVM
            {
                Nome = email,
                IsComprador = user.Comprador != null,
                IsVendedor = user.Vendedor != null,

                FavoritosCount = idComprador == 0 ? 0 :
                    await _db.Favoritos.CountAsync(f => f.IdComprador == idComprador),

                VisitasCount = idComprador == 0 ? 0 :
                    await _db.Visita.CountAsync(v => v.IdComprador == idComprador),

                ReservasCount = idComprador == 0 ? 0 :
                    await _db.Reservas.CountAsync(r => r.IdComprador == idComprador),

                ComprasCount = idComprador == 0 ? 0 :
                    await _db.Compras.CountAsync(c => c.IdComprador == idComprador),

                AnunciosCount = idVendedor == 0 ? 0 :
                    await _db.Anuncios.CountAsync(a => a.IdVendedor == idVendedor)
            };

            var atividade = new List<PainelAtividadeVM>();
            var limite = DateTime.Now.AddDays(-30);

            // FAVORITOS
            atividade.AddRange(
                await _db.Favoritos
                    .Where(f => f.IdComprador == idComprador &&
                                f.DataFavorito >= limite)
                    .Include(f => f.Anuncio)
                    .Select(f => new PainelAtividadeVM
                    {
                        Tipo = "Favorito",
                        Titulo = f.Anuncio.Titulo,
                        Data = f.DataFavorito
                    })
                    .ToListAsync()
            );

            // RESERVAS
            atividade.AddRange(
                await _db.Reservas
                    .Where(r => r.IdComprador == idComprador &&
                                r.DataReserva >= limite)
                    .Include(r => r.IdAnuncioNavigation)
                    .Select(r => new PainelAtividadeVM
                    {
                        Tipo = "Reserva",
                        Titulo = r.IdAnuncioNavigation.Titulo,
                        Data = r.DataReserva
                    })
                    .ToListAsync()
            );

            // VISITAS
            atividade.AddRange(
                await _db.Visita
                    .Where(v => v.IdComprador == idComprador &&
                                v.DataHora >= limite)
                    .Include(v => v.IdAnuncioNavigation)
                    .Select(v => new PainelAtividadeVM
                    {
                        Tipo = "Visita",
                        Titulo = v.IdAnuncioNavigation.Titulo,
                        Data = v.DataHora
                    })
                    .ToListAsync()
            );

            // COMPRAS
            atividade.AddRange(
                await _db.Compras
                    .Where(c => c.IdComprador == idComprador)
                    .Include(c => c.IdAnuncioNavigation)
                    .Select(c => new PainelAtividadeVM
                    {
                        Tipo = "Compra",
                        Titulo = c.IdAnuncioNavigation.Titulo,
                        Data = c.DataCompra
                    })
                    .ToListAsync()
            );

            vm.Atividade = atividade
                .OrderByDescending(a => a.Data)
                .Take(6)
                .ToList();

            return View(vm);
        }
    }
}

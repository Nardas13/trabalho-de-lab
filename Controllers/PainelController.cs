using AutoHubProjeto.Models;
using AutoHubProjeto.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
                    await _db.Visita.CountAsync(v =>
                        v.IdComprador == idComprador &&
                        v.Estado == "confirmada" &&
                        v.DataHora > DateTime.Now),


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
                    .Where(v =>
                        v.IdComprador == idComprador &&
                        v.Estado == "confirmada" &&
                        v.DataHora > DateTime.Now)
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
        public async Task<IActionResult> MinhasVisitas()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.Identity.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Comprador == null)
                return RedirectToAction("Index");

            int id = user.Comprador.IdComprador;
            var agora = DateTime.Now;

            var visitas = await _db.Visita
                .Where(v => v.IdComprador == id)
                .Include(v => v.IdAnuncioNavigation)
                .Include(v => v.IdAnuncioNavigation.AnuncioImagems)
                .OrderBy(v => v.DataHora)
                .ToListAsync();

            var vm = new MinhasVisitasVM();

            foreach (var v in visitas)
            {
                string estadoTexto = v.Estado switch
                {
                    "pendente" => "Pendente",
                    "confirmada" => "Confirmada",
                    "cancelada" => "Cancelada",
                    "realizada" => "Concluída",
                    _ => "Indefinido"
                };

                var item = new MinhasVisitasItemVM
                {
                    IdVisita = v.IdVisita,
                    Titulo = v.IdAnuncioNavigation.Titulo,
                    Imagem = v.IdAnuncioNavigation.AnuncioImagems.FirstOrDefault()?.Url ?? "imgs/carros.jpg",
                    DataHora = v.DataHora,
                    Estado = estadoTexto
                };

                if (
                    (v.Estado == "pendente" || v.Estado == "confirmada") &&
                    v.DataHora > agora
                )
                {
                    vm.Futuras.Add(item);
                }
                else if (v.Estado == "realizada" || v.Estado == "cancelada")
                {
                    vm.Passadas.Add(item);
                }
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult Cancelar([FromBody] JsonElement dados)
        {
            int idVisita = dados.GetProperty("idVisita").GetInt32();

            var visita = _db.Visita.FirstOrDefault(v => v.IdVisita == idVisita);

            if (visita == null)
                return Json(new { ok = false, msg = "Visita não encontrada." });

            visita.Estado = "cancelada";
            _db.SaveChanges();

            return Json(new { ok = true, msg = "Visita cancelada com sucesso." });
        }

    }
}

using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.Controllers
{
    public class FavoritosController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FavoritosController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Toggle(int idAnuncio)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { ok = false, mensagem = "login" });

            var email = User.Identity.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Comprador == null)
                return Json(new { ok = false, mensagem = "nocomprador" });

            var compradorId = user.Comprador.IdComprador;

            var existente = await _db.Favoritos
                .FirstOrDefaultAsync(f => f.IdComprador == compradorId && f.IdAnuncio == idAnuncio);

            if (existente != null)
            {
                _db.Favoritos.Remove(existente);
                await _db.SaveChangesAsync();

                int total = await _db.Favoritos.CountAsync(f => f.IdComprador == compradorId);

                return Json(new { ok = true, estado = "removido", total });
            }

            var novo = new Favorito
            {
                IdComprador = compradorId,
                IdAnuncio = idAnuncio
            };

            _db.Favoritos.Add(novo);
            await _db.SaveChangesAsync();

            int totalAdd = await _db.Favoritos.CountAsync(f => f.IdComprador == compradorId);

            return Json(new { ok = true, estado = "adicionado", total = totalAdd });
        }

    }
}

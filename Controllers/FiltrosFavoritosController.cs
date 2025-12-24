using System.Text.Json;
using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.Controllers
{
    public class FiltrosFavoritosController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FiltrosFavoritosController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult Guardar([FromBody] JsonElement dados)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            var email = User.Identity.Name;

            var comprador = _db.Utilizadors
                .Include(u => u.Comprador)
                .FirstOrDefault(u => u.Email == email)
                ?.Comprador;

            if (comprador == null)
                return Unauthorized();

            var filtrosDict = JsonSerializer
                .Deserialize<Dictionary<string, string>>(dados.GetProperty("filtros").GetRawText())!;

            var filtrosNormalizadosDict = filtrosDict
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .OrderBy(kv => kv.Key)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            var filtrosNormalizados = JsonSerializer.Serialize(filtrosNormalizadosDict);

            bool filtroDuplicado = _db.FiltroFavoritos.Any(f =>
                f.IdComprador == comprador.IdComprador &&
                f.FiltrosJson == filtrosNormalizados
            );

            if (filtroDuplicado)
                return Json(new { ok = false, msg = "Este conjunto de filtros já está guardado." });

            var filtro = new FiltroFavorito
            {
                IdComprador = comprador.IdComprador,
                Nome = $"Filtro {DateTime.Now:dd/MM HH:mm}",
                FiltrosJson = filtrosNormalizados
            };

            _db.FiltroFavoritos.Add(filtro);
            _db.SaveChanges();

            return Json(new { ok = true });
        }


        [HttpGet]
        public IActionResult Aplicar(int id)
        {
            var filtro = _db.FiltroFavoritos.Find(id);
            if (filtro == null) return NotFound();

            string qs = "";

            try
            {
                string json = filtro.FiltrosJson?.Trim() ?? "";

                if (json.StartsWith("\"") && json.EndsWith("\""))
                    json = JsonSerializer.Deserialize<string>(json) ?? "";

                var query = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                            ?? new Dictionary<string, string>();

                qs = string.Join("&", query
                    .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                    .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
            }
            catch
            {
                // se algo correr mal, seguimos com qs vazio
            }

            return Redirect(
                string.IsNullOrEmpty(qs)
                    ? $"/Anuncios?filtroAtivo={id}"
                    : $"/Anuncios?{qs}&filtroAtivo={id}"
            );
        }

        [HttpPost]
        public IActionResult Remover([FromBody] JsonElement dados)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            var prop = dados.GetProperty("idFiltro");

            int idFiltro = prop.ValueKind == JsonValueKind.Number
                ? prop.GetInt32()
                : int.Parse(prop.GetString()!);

            var email = User.Identity.Name;

            var comprador = _db.Utilizadors
                .Include(u => u.Comprador)
                .FirstOrDefault(u => u.Email == email)
                ?.Comprador;

            if (comprador == null)
                return Unauthorized();

            var filtro = _db.FiltroFavoritos
                .FirstOrDefault(f => f.IdFiltro == idFiltro &&
                                     f.IdComprador == comprador.IdComprador);

            if (filtro == null)
                return Json(new { ok = false, msg = "Filtro não encontrado." });

            _db.FiltroFavoritos.Remove(filtro);
            _db.SaveChanges();

            return Json(new { ok = true, msg = "Filtro removido." });
        }

    }
}
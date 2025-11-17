using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.Controllers
{
    public class AnunciosController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AnunciosController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(
            string? Categoria,
            string? Marca,
            int? AnoMin,
            int? AnoMax,
            decimal? PrecoMin,
            decimal? PrecoMax,
            string? Combustivel,
            int? KmMax,
            string? Caixa,
            string? Ordenar
        )
        {
            // Query base
            var query = _db.Anuncios
                .Include(a => a.IdVeiculoNavigation)
                .Include(a => a.AnuncioImagems)
                .AsQueryable();

            // -------- FILTROS --------
            if (!string.IsNullOrEmpty(Categoria))
                query = query.Where(a => a.IdVeiculoNavigation.Categoria == Categoria);

            if (!string.IsNullOrEmpty(Marca))
                query = query.Where(a => a.IdVeiculoNavigation.Marca == Marca);

            if (AnoMin.HasValue)
                query = query.Where(a => a.IdVeiculoNavigation.Ano >= AnoMin.Value);

            if (AnoMax.HasValue)
                query = query.Where(a => a.IdVeiculoNavigation.Ano <= AnoMax.Value);

            if (PrecoMin.HasValue)
                query = query.Where(a => a.Preco >= PrecoMin.Value);

            if (PrecoMax.HasValue)
                query = query.Where(a => a.Preco <= PrecoMax.Value);

            if (!string.IsNullOrEmpty(Combustivel))
                query = query.Where(a => a.IdVeiculoNavigation.Combustivel == Combustivel);

            if (KmMax.HasValue)
                query = query.Where(a => a.IdVeiculoNavigation.Quilometragem <= KmMax.Value);

            if (!string.IsNullOrEmpty(Caixa))
                query = query.Where(a => a.IdVeiculoNavigation.Caixa == Caixa);

            // -------- ORDENAR --------
            Ordenar ??= "relevancia";

            query = Ordenar switch
            {
                "preco-desc" => query.OrderByDescending(a => a.Preco),
                "preco-asc" => query.OrderBy(a => a.Preco),
                "ano-desc" => query.OrderByDescending(a => a.IdVeiculoNavigation.Ano),
                "ano-asc" => query.OrderBy(a => a.IdVeiculoNavigation.Ano),
                "km-asc" => query.OrderBy(a => a.IdVeiculoNavigation.Quilometragem),
                "km-desc" => query.OrderByDescending(a => a.IdVeiculoNavigation.Quilometragem),
                _ => query.OrderBy(a => a.IdAnuncio) // relevância default
            };

            // -------- VIEWMODEL --------
            var vm = new FiltroAnunciosViewModel
            {
                Resultados = query.ToList(),

                Categoria = Categoria,
                Marca = Marca,
                AnoMin = AnoMin,
                AnoMax = AnoMax,
                PrecoMin = PrecoMin,
                PrecoMax = PrecoMax,
                Combustivel = Combustivel,
                KmMax = KmMax,
                Caixa = Caixa,
                Ordenar = Ordenar,

                Categorias = _db.Veiculos
                    .Select(v => v.Categoria)
                    .Where(c => c != null && c != "")
                    .Distinct()
                    .ToList(),

                Marcas = _db.Veiculos
                    .Select(v => v.Marca)
                    .Where(m => m != null && m != "")
                    .Distinct()
                    .ToList()
            };

            return View(vm);
        }
    }
}

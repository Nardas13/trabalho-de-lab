using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.Controllers.Admin
{
    public class AdmDashboardController : AdminBaseController
    {
        public AdmDashboardController(ApplicationDbContext db)
            : base(db) { }

        public async Task<IActionResult> Index()
        {
            // Contagens
            var numCompradores = await _db.Compradors.CountAsync();

            var numVendedores = await _db.Vendedors
                .Where(v => v.Aprovado)
                .CountAsync();

            var numAnunciosAtivos = await _db.Anuncios
                .Where(a => a.Estado == "ativo")
                .CountAsync();

            var numVendas = await _db.Compras
                .Where(c => c.EstadoPagamento == "pago")
                .CountAsync();

            // Top marcas
            var topMarcas = await _db.Veiculos
                .Where(v => v.Marca != null)
                .GroupBy(v => v.Marca)
                .Select(g => new
                {
                    Marca = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToListAsync();

            // Top modelos
            var topModelos = await _db.Veiculos
                .Where(v => v.Modelo != null)
                .GroupBy(v => v.Modelo)
                .Select(g => new
                {
                    Modelo = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToListAsync();

            ViewBag.Compradores = numCompradores;
            ViewBag.Vendedores = numVendedores;
            ViewBag.Anuncios = numAnunciosAtivos;
            ViewBag.Vendas = numVendas;
            ViewBag.TopMarcas = topMarcas;
            ViewBag.TopModelos = topModelos;

            return View();
        }
    }
}

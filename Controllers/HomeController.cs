using AutoHubProjeto.Helpers;
using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AutoHubProjeto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            // garante que reservas expiradas libertam anúncios
            ReservaHelper.ExpirarReservasAsync(_db).Wait();

            var destaques = _db.Anuncios
            .Include(a => a.IdVeiculoNavigation)
            .Include(a => a.AnuncioImagems)
            .Where(a =>
                a.Estado == "ativo" ||
                a.Estado == "reservado"
            )
            .OrderByDescending(a => a.DataPublicacao)
            .Take(3)
            .ToList();

            return View(destaques);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

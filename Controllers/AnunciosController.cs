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

        public IActionResult Index()
        {
            var anuncios = _db.Anuncios
                .Include(a => a.IdVeiculoNavigation)
                .Include(a => a.AnuncioImagems)
                .ToList();

            return View(anuncios);
        }
    }
}

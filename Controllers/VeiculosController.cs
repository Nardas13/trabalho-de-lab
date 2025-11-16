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

            return View(anuncio);
        }
    }
}

using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoHubProjeto.Controllers
{
    public class TesteController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TesteController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var veiculos = _db.Veiculos.ToList();
            return Json(veiculos);
        }
    }
}

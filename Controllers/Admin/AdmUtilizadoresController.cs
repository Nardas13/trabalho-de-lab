using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdmUtilizadoresController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdmUtilizadoresController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var utilizadores = _context.Utilizadors
            .Include(u => u.Comprador)
            .Include(u => u.Vendedor)
            .Include(u => u.Administrador)
            .ToList();

        return View(utilizadores);
    }

    [HttpPost]
    public IActionResult Bloquear(int id, string motivo)
    {
        var user = _context.Utilizadors.Find(id);
        if (user == null) return NotFound();

        user.EstadoConta = "Bloqueado";

//motivo 
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Desbloquear(int id)
    {
        var user = _context.Utilizadors.Find(id);
        if (user == null) return NotFound();

        user.EstadoConta = "Ativo";
        user.MotivoBloqueio = null; 
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}

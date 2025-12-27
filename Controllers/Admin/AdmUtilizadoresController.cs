using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdmUtilizadoresController : Controller
{
    private readonly ApplicationDbContext _context;

    private const string ADMIN_SUPREMO_EMAIL = "autohubadm1@gmail.com";
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

        if (user.Email == ADMIN_SUPREMO_EMAIL)
        {
            return BadRequest("Não é possível bloquear o administrador supremo.");
        }

        user.EstadoConta = "Bloqueado";
        user.MotivoBloqueio = motivo?.Trim();

        _context.SaveChanges();

        return RedirectToAction("Index");
    }


    [HttpPost]
    public IActionResult Desbloquear(int id, string motivo)
    {
        var user = _context.Utilizadors.Find(id);
        if (user == null) return NotFound();

        user.EstadoConta = "Ativo";
        user.MotivoBloqueio = motivo?.Trim();

        _context.SaveChanges();

        return RedirectToAction("Index");
    }

}


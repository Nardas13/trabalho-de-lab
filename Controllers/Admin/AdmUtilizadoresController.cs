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

        user.EstadoConta = "ativo";
        user.MotivoBloqueio = null; 

        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Detalhes(int id)
    {
        var user = _context.Utilizadors
            .Include(u => u.Comprador)
            .Include(u => u.Vendedor)
            .Include(u => u.Administrador)
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    public IActionResult Atualizar(Utilizador model)
    {
        var user = _context.Utilizadors.Find(model.Id);
        if (user == null) return NotFound();

        user.Nome = model.Nome?.Trim();
        user.Email = model.Email?.Trim();

        _context.SaveChanges();

        return RedirectToAction("Detalhes", new { id = user.Id });
    }

    [HttpPost]
    public IActionResult AprovarVendedor(int id)
    {
        var user = _context.Utilizadors
            .Include(u => u.Vendedor)
            .FirstOrDefault(u => u.Id == id);

        if (user == null || user.Vendedor == null)
            return NotFound();

        user.Vendedor.Aprovado = true;
        user.Vendedor.DataAprovacao = DateTime.Now;

        var adminEmail = User.Identity!.Name;
        var admin = _context.Utilizadors.FirstOrDefault(u => u.Email == adminEmail);

        if (admin != null)
            user.Vendedor.IdAdminAprovador = admin.Id;

        _context.SaveChanges();

        return RedirectToAction("Detalhes", new { id });
    }

    [HttpPost]
    public IActionResult RejeitarVendedor(int id)
    {
        var user = _context.Utilizadors
            .Include(u => u.Vendedor)
            .FirstOrDefault(u => u.Id == id);

        if (user == null || user.Vendedor == null)
            return NotFound();

        _context.Vendedors.Remove(user.Vendedor);
        _context.SaveChanges();

        return RedirectToAction("Detalhes", new { id });
    }

}


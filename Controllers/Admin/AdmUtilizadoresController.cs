using AutoHubProjeto.Controllers.Admin;
using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdmUtilizadoresController : AdmBaseController
{
    private const string ADMIN_SUPREMO_EMAIL = "autohubadm1@gmail.com";
    public AdmUtilizadoresController(ApplicationDbContext context)
        : base(context)
    {
    }

    public IActionResult Index()
    {
        var utilizadores = _db.Utilizadors
            .Include(u => u.Comprador)
            .Include(u => u.Vendedor)
            .Include(u => u.Administrador)
            .ToList();

        return View(utilizadores);
    }

    public IActionResult Detalhes(int id)
    {
        var user = _db.Utilizadors
            .Include(u => u.Comprador)
            .Include(u => u.Vendedor)
            .Include(u => u.Administrador)
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        return View(user);
    }


    [HttpPost]
    public IActionResult Bloquear(int id, string motivo)
    {
        var user = _db.Utilizadors
            .Include(u => u.Administrador)
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        if (user.Administrador != null)
        {
            TempData["Erro"] = "Não é possível bloquear um administrador.";
            return RedirectToAction("Index");
        }

        user.EstadoConta = "Bloqueado";
        user.MotivoBloqueio = motivo?.Trim();

        _db.SaveChanges();

        var adminEmail = User.Identity!.Name;
        var admin = _db.Utilizadors.FirstOrDefault(u => u.Email == adminEmail);

        if (admin != null)
        {
            RegistarLog(
                admin.Id,
                "Bloqueou utilizador",
                user.Email,            
                user.Id.ToString(),
                motivo
            );

        }

        return RedirectToAction("Index");
    }


    [HttpPost]
    public IActionResult Desbloquear(int id)
    {
        var user = _db.Utilizadors.Find(id);
        if (user == null) return NotFound();

        user.EstadoConta = "ativo";
        user.MotivoBloqueio = null;

        _db.SaveChanges();

        var adminEmail = User.Identity!.Name;
        var admin = _db.Utilizadors.FirstOrDefault(u => u.Email == adminEmail);

        if (admin != null)
        {
            RegistarLog(
                admin.Id,
                "Desbloqueou utilizador",
                user.Email,
                user.Id.ToString()
            );

        }

        return RedirectToAction("Index");
    }

    //[HttpPost]
    //public IActionResult Atualizar(Utilizador model)
    //{
    //    var user = _db.Utilizadors.Find(model.Id);
    //    if (user == null) return NotFound();

    //    user.Nome = model.Nome?.Trim();
    //    user.Email = model.Email?.Trim();

    //    _db.SaveChanges();

    //    return RedirectToAction("Detalhes", new { id = user.Id });
    //}

    [HttpPost]
    public IActionResult AprovarVendedor(int id)
    {
        var user = _db.Utilizadors
            .Include(u => u.Vendedor)
            .FirstOrDefault(u => u.Id == id);

        if (user == null || user.Vendedor == null)
            return NotFound();

        user.Vendedor.Aprovado = true;
        user.Vendedor.DataAprovacao = DateTime.Now;

        var adminEmail = User.Identity!.Name;
        var admin = _db.Utilizadors.FirstOrDefault(u => u.Email == adminEmail);

        if (admin != null)
        {
            user.Vendedor.IdAdminAprovador = admin.Id;

            RegistarLog(
                admin.Id,
                "Aprovou vendedor",
                user.Email,
                user.Id.ToString()
            );

        }

        _db.SaveChanges();

        return RedirectToAction("Detalhes", new { id });
    }

    [HttpPost]
    public IActionResult RejeitarVendedor(int id)
    {
        var user = _db.Utilizadors
            .Include(u => u.Vendedor)
            .FirstOrDefault(u => u.Id == id);

        if (user == null || user.Vendedor == null)
            return NotFound();

        var adminEmail = User.Identity!.Name;
        var admin = _db.Utilizadors.FirstOrDefault(u => u.Email == adminEmail);

        if (admin != null)
        {
            RegistarLog(
                admin.Id,
                "Rejeitou vendedor",
                user.Email,
                user.Id.ToString()
            );

        }

        _db.Vendedors.Remove(user.Vendedor);
        _db.SaveChanges();

        return RedirectToAction("Detalhes", new { id });
    }

}


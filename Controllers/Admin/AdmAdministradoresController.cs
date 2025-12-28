using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace AutoHubProjeto.Controllers.Admin
{
    public class AdmAdministradoresController : AdmBaseController
    {
        public AdmAdministradoresController(ApplicationDbContext db) : base(db) { }

        // LISTA DE ADMINISTRADORES
        public IActionResult Index()
        {
            var admins = _db.Utilizadors
                .Include(u => u.Administrador)
                .Where(u => u.Administrador != null)
                .ToList();

            return View(admins);
        }

        // FORM 
        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        // CRIAR ADMIN
        [HttpPost]
        public IActionResult Criar(string Nome, string Email)
        {
            if (_db.Utilizadors.Any(u => u.Email == Email))
            {
                TempData["Erro"] = "Já existe um utilizador com esse email.";
                return RedirectToAction("Index");
            }

            // password temporária aleatória 
            var tempPassword = Guid.NewGuid().ToString();
            var tempHashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(tempPassword));
            var tempHash = Convert.ToBase64String(tempHashBytes);
            var tempUsername = Email.Trim().ToLower(); 

            var user = new Utilizador
            {
                Nome = Nome.Trim(),
                Email = Email.Trim(),
                EstadoConta = "ativo",
                Username = tempUsername,
                PasswordHash = tempHashBytes
            };

            _db.Utilizadors.Add(user);
            _db.SaveChanges();

            var admin = new Administrador
            {
                IdAdmin = user.Id
            };

            _db.Administradors.Add(admin);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}

using AutoHubProjeto.Models;
using AutoHubProjeto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AutoHubProjeto.Controllers.Admin
{
    public class AdmAdministradoresController : AdmBaseController
    {
        private readonly IEmailService _emailService;

        public AdmAdministradoresController(
            ApplicationDbContext db,
            IEmailService emailService
        ) : base(db)
        {
            _emailService = emailService;
        }

        // ===============================
        // LISTAR ADMINISTRADORES
        // ===============================
        public IActionResult Index()
        {
            var admins = _db.Utilizadors
                .Include(u => u.Administrador)
                .Where(u => u.Administrador != null)
                .ToList();

            return View(admins);
        }

        // ===============================
        // CRIAR ADMINISTRADOR (POST)
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Criar(string Nome, string Email)
        {
            if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Email))
            {
                TempData["Erro"] = "Nome e Email são obrigatórios.";
                return RedirectToAction("Index");
            }

            if (_db.Utilizadors.Any(u => u.Email == Email))
            {
                TempData["Erro"] = "Já existe um utilizador com esse email.";
                return RedirectToAction("Index");
            }

            // gerar credenciais
            string username = "admin_" + Guid.NewGuid().ToString("N")[..8];
            string plainPassword = Guid.NewGuid().ToString("N")[..10];
            byte[] passwordHash = HashPassword(plainPassword);

            var user = new Utilizador
            {
                Nome = Nome.Trim(),
                Email = Email.Trim(),
                Username = username,
                EstadoConta = "ativo",
                PasswordHash = passwordHash
            };

            _db.Utilizadors.Add(user);
            _db.SaveChanges();

            var admin = new Administrador
            {
                IdAdmin = user.Id
            };

            _db.Administradors.Add(admin);
            _db.SaveChanges();

            // enviar email com credenciais
            await _emailService.SendEmailAsync(
                Email,
                "Conta de Administrador AutoHub",
                $@"
                    <h2>Conta de Administrador criada</h2>

                    <p>A tua conta de administrador foi criada com sucesso.</p>

                    <p><strong>Username:</strong> {username}</p>
                    <p><strong>Password:</strong> {plainPassword}</p>

                    <p>
                        Por motivos de segurança, recomenda-se a alteração da password
                        após o primeiro login.
                    </p>
                "
            );

            TempData["Sucesso"] = "Administrador criado e email enviado.";
            return RedirectToAction("Index");
        }

        // ===============================
        // HASH DE PASSWORD
        // ===============================
        private byte[] HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}

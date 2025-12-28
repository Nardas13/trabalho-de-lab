using AutoHubProjeto.Models;
using AutoHubProjeto.Services;
using AutoHubProjeto.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        // CRIAR ADMINISTRADOR
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

            byte[] passwordHash = PasswordHelper.HashPassword(plainPassword);

            var user = new Utilizador
            {
                Nome = Nome.Trim(),
                Email = Email.Trim(),
                Username = username,
                EstadoConta = "ativo",
                EmailConfirmado = true,
                DataCriacao = DateTime.Now,
                PasswordHash = passwordHash
            };

            _db.Utilizadors.Add(user);
            _db.SaveChanges();

            _db.Compradors.Add(new Comprador
            {
                IdComprador = user.Id,
            });

            _db.Administradors.Add(new Administrador
            {
                IdAdmin = user.Id
            });

            _db.SaveChanges();

            var html = EmailTemplates.AdminAccountCreated(username, plainPassword);

            await _emailService.SendEmailAsync(
                Email,
                "Conta de Administrador AutoHub",
                html
            );

            TempData["Sucesso"] = "Administrador criado e email enviado.";
            return RedirectToAction("Index");
        }
    }
}

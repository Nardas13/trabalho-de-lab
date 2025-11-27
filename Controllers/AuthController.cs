using AutoHubProjeto.Models;
using AutoHubProjeto.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.Json;

namespace AutoHubProjeto.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _email;

        public AuthController(ApplicationDbContext db, IEmailService email)
        {
            _db = db;
            _email = email;
        }

        public IActionResult Index() => View();

        // ---------------------------------------------------------
        // LOGIN
        // ---------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = await _db.Utilizadors.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null || !PasswordHelper.VerifyPassword(Password, user.PasswordHash))
            {
                TempData["AuthError"] = "Email ou password inválidos.";
                TempData["AuthErrorSource"] = "login";
                return RedirectToAction("Index");
            }

            if (user.EstadoConta == "bloqueado")
            {
                TempData["AuthError"] = "A tua conta encontra-se bloqueada.";
                return RedirectToAction("Index");
            }

            if (!user.EmailConfirmado)
            {
                TempData["AuthError"] = "Confirma o teu email antes de iniciar sessão.";
                TempData["EmailConfirm"] = user.Email;
                return RedirectToAction("ConfirmEmail");
            }

            TempData["AuthSuccess"] = "Sessão iniciada com sucesso!";
            return RedirectToAction("Index", "Home");
        }

        // ---------------------------------------------------------
        // REGISTO – PASSO 1
        // ---------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Register(
            string Nome, string Email, string Username, string Contacto,
            string Password, string ConfirmPassword, string Morada)
        {
            if (Password != ConfirmPassword)
            {
                TempData["AuthError"] = "As passwords não coincidem.";
                TempData["AuthErrorSource"] = "register";
                return RedirectToAction("Index");
            }

            if (await _db.Utilizadors.AnyAsync(u => u.Email == Email))
            {
                TempData["AuthError"] = "Este email já está registado.";
                TempData["AuthErrorSource"] = "register";
                return RedirectToAction("Index");
            }

            if (await _db.Utilizadors.AnyAsync(u => u.Username == Username))
            {
                TempData["AuthError"] = "Este username já está em uso.";
                TempData["AuthErrorSource"] = "register";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrWhiteSpace(Contacto) &&
                await _db.Utilizadors.AnyAsync(u => u.Telefone == Contacto))
            {
                TempData["AuthError"] = "Este contacto já está associado a outra conta.";
                TempData["AuthErrorSource"] = "register";
                return RedirectToAction("Index");
            }

            var codigo = GenerateCode();

            var pendente = new PendingUser
            {
                Nome = Nome,
                Email = Email,
                Username = Username,
                Contacto = Contacto,
                Morada = Morada,
                PasswordHash = Convert.ToBase64String(PasswordHelper.HashPassword(Password))
            };

            HttpContext.Session.SetString("PendingUser", JsonSerializer.Serialize(pendente));
            HttpContext.Session.SetString("PendingCode", codigo);

            await _email.SendVerificationEmail(Email, codigo);

            TempData["EmailConfirm"] = Email;
            TempData["AuthSuccess"] = "Enviámos um código para o teu email.";
            return RedirectToAction("ConfirmEmail");
        }

        // ---------------------------------------------------------
        // REGISTO – PASSO 2
        // ---------------------------------------------------------
        [HttpGet]
        public IActionResult ConfirmEmail()
        {
            var email = TempData["EmailConfirm"] as string
                        ?? Request.Query["email"].ToString();

            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Index");

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string Email, string Codigo)
        {
            var codigoGuardado = HttpContext.Session.GetString("PendingCode");
            var dadosUser = HttpContext.Session.GetString("PendingUser");

            if (codigoGuardado == null || dadosUser == null)
            {
                TempData["AuthError"] = "Registo expirado. Tenta novamente.";
                return RedirectToAction("Index");
            }

            if (Codigo != codigoGuardado)
            {
                TempData["AuthError"] = "Código incorreto.";
                TempData["EmailConfirm"] = Email;
                return RedirectToAction("ConfirmEmail");
            }

            var tempUser = JsonSerializer.Deserialize<PendingUser>(dadosUser);

            var user = new Utilizador
            {
                Nome = tempUser.Nome,
                Email = tempUser.Email,
                Username = tempUser.Username,
                Telefone = tempUser.Contacto,
                Morada = tempUser.Morada,

                
                PasswordHash = Convert.FromBase64String(tempUser.PasswordHash),

                DataCriacao = DateTime.Now,
                EstadoConta = "ativo",
                EmailConfirmado = true
            };

            _db.Utilizadors.Add(user);
            await _db.SaveChangesAsync();

            HttpContext.Session.Remove("PendingUser");
            HttpContext.Session.Remove("PendingCode");

            TempData["AuthSuccess"] = "Conta criada com sucesso!";
            return RedirectToAction("Index", "Home");
        }

        // ---------------------------------------------------------
        // REENVIAR CÓDIGO
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> ResendCode(string email)
        {
            var codigo = HttpContext.Session.GetString("PendingCode");
            if (codigo == null)
            {
                TempData["AuthError"] = "O registo expirou.";
                return RedirectToAction("Index");
            }

            await _email.SendVerificationEmail(email, codigo);

            TempData["EmailConfirm"] = email;
            TempData["AuthSuccess"] = "Novo código reenviado!";
            return RedirectToAction("ConfirmEmail");
        }

        // ---------------------------------------------------------
        // FORGOT / RESET
        // ---------------------------------------------------------
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            var user = await _db.Utilizadors.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null || !user.EmailConfirmado)
            {
                TempData["AuthError"] =
                    "Se o email existir e estiver confirmado, receberás um código.";
                return RedirectToAction("ForgotPassword");
            }

            var code = GenerateCode();
            user.ResetPasswordCode = code;
            await _db.SaveChangesAsync();

            await _email.SendResetPasswordEmail(Email, code);

            TempData["ResetEmail"] = Email;
            TempData["AuthSuccess"] = "Código enviado!";
            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var email = TempData["ResetEmail"] as string
                        ?? Request.Query["email"].ToString();

            if (email == null)
                return RedirectToAction("Index");

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(
            string Email, string Codigo, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                TempData["AuthError"] = "As passwords não coincidem.";
                TempData["ResetEmail"] = Email;
                return RedirectToAction("ResetPassword");
            }

            var user = await _db.Utilizadors
                .FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null || user.ResetPasswordCode != Codigo)
            {
                TempData["AuthError"] = "Código inválido.";
                TempData["ResetEmail"] = Email;
                return RedirectToAction("ResetPassword");
            }

            user.PasswordHash = PasswordHelper.HashPassword(Password);
            user.ResetPasswordCode = null;

            await _db.SaveChangesAsync();

            TempData["AuthSuccess"] = "Password alterada!";
            return RedirectToAction("Index");
        }

        private static string GenerateCode() =>
            RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }
}

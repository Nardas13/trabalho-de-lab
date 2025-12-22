using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AutoHubProjeto.Controllers
{
    public class ComprasController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ComprasController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ====================================================
        // CRIAR COMPRA
        // ====================================================
        [HttpPost]
        public IActionResult Criar([FromBody] JsonElement dados)
        {
            if (!User.Identity!.IsAuthenticated)
                return Json(new { ok = false, msg = "Precisas de iniciar sessão." });

            int idAnuncio = dados.GetProperty("idAnuncio").GetInt32();
            string metodo = dados.GetProperty("metodoPagamento").GetString()!;

            var email = User.Identity.Name;

            var user = _db.Utilizadors
                .Include(u => u.Comprador)
                .Include(u => u.Vendedor)
                .FirstOrDefault(u => u.Email == email);

            if (user?.Comprador == null)
                return Json(new { ok = false, msg = "Apenas compradores podem comprar." });

            var anuncio = _db.Anuncios.FirstOrDefault(a => a.IdAnuncio == idAnuncio);
            if (anuncio == null)
                return Json(new { ok = false, msg = "Anúncio não encontrado." });

            if (anuncio.Estado != "ativo" && anuncio.Estado != "reservado")
                return Json(new { ok = false, msg = "Este veículo não está disponível." });

            // bloquear compra pendente duplicada
            bool compraPendente = _db.Compras.Any(c =>
                c.IdAnuncio == idAnuncio &&
                c.IdComprador == user.Comprador.IdComprador &&
                c.EstadoPagamento == "pendente");

            if (compraPendente)
                return Json(new { ok = false, msg = "Já tens um pedido de compra pendente para este veículo." });

            // bloquear se já foi vendido (comentado a pedido)
            //bool vendido = _db.Compras.Any(c =>
            //    c.IdAnuncio == idAnuncio &&
            //    c.EstadoPagamento == "pago");
            //
            //if (vendido)
            //    return Json(new { ok = false, msg = "Este veículo já foi vendido." });

            // bloquear vendedor a comprar
            if (user.Vendedor != null && user.Vendedor.IdVendedor == anuncio.IdVendedor)
                return Json(new { ok = false, msg = "Não podes comprar o teu próprio veículo." });

            var compra = new Compra
            {
                IdAnuncio = idAnuncio,
                IdComprador = user.Comprador.IdComprador,
                Valor = anuncio.Preco,
                DataCompra = DateTime.Now,
                EstadoPagamento = "pendente",
                ReferenciaPagamento = metodo
            };

            _db.Compras.Add(compra);
            _db.SaveChanges();

            return Json(new
            {
                ok = true,
                msg = "Pedido de compra enviado. O vendedor será notificado."
            });
        }

        [HttpPost]
        public IActionResult Confirmar(int idCompra)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            var email = User.Identity.Name;

            var vendedor = _db.Utilizadors
                .Include(u => u.Vendedor)
                .FirstOrDefault(u => u.Email == email)
                ?.Vendedor;

            if (vendedor == null)
                return Unauthorized();

            var compra = _db.Compras
                .Include(c => c.IdAnuncioNavigation)
                .FirstOrDefault(c => c.IdCompra == idCompra);

            if (compra == null)
                return Json(new { ok = false, msg = "Compra não encontrada." });

            if (compra.IdAnuncioNavigation.IdVendedor != vendedor.IdVendedor)
                return Unauthorized();

            compra.EstadoPagamento = "pago";
            compra.IdAnuncioNavigation.Estado = "vendido";

            // cancelar visitas pendentes
            var visitas = _db.Visita.Where(v =>
                v.IdAnuncio == compra.IdAnuncio &&
                v.Estado == "pendente");

            foreach (var v in visitas)
                v.Estado = "cancelada";

            _db.SaveChanges();

            return Json(new { ok = true, msg = "Compra confirmada." });
        }

        [HttpPost]
        public IActionResult Recusar(int idCompra)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            var email = User.Identity.Name;

            var vendedor = _db.Utilizadors
                .Include(u => u.Vendedor)
                .FirstOrDefault(u => u.Email == email)
                ?.Vendedor;

            if (vendedor == null)
                return Unauthorized();

            var compra = _db.Compras
                .Include(c => c.IdAnuncioNavigation)
                .FirstOrDefault(c => c.IdCompra == idCompra);

            if (compra == null)
                return Json(new { ok = false, msg = "Compra não encontrada." });

            if (compra.IdAnuncioNavigation.IdVendedor != vendedor.IdVendedor)
                return Unauthorized();

            compra.EstadoPagamento = "cancelado";
            _db.SaveChanges();

            return Json(new { ok = true, msg = "Compra cancelada." });
        }

    }
}

using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AutoHubProjeto.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReservasController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ====================================================
        // CRIAR RESERVA
        // ====================================================
        [HttpPost]
        public IActionResult Criar([FromBody] JsonElement dados)
        {
            if (!User.Identity!.IsAuthenticated)
                return Json(new { ok = false, msg = "Precisas de iniciar sessão." });

            int idAnuncio = dados.GetProperty("idAnuncio").GetInt32();

            var horasProp = dados.GetProperty("horas");

            int horas;
            if (horasProp.ValueKind == JsonValueKind.Number)
            {
                horas = horasProp.GetInt32();
            }
            else if (horasProp.ValueKind == JsonValueKind.String)
            {
                if (!int.TryParse(horasProp.GetString(), out horas))
                    return Json(new { ok = false, msg = "Duração inválida." });
            }
            else
            {
                return Json(new { ok = false, msg = "Duração inválida." });
            }


            if (horas != 24 && horas != 48 && horas != 72)
                return Json(new { ok = false, msg = "Duração inválida." });

            var email = User.Identity!.Name;

            var user = _db.Utilizadors
                .Include(u => u.Comprador)
                .Include(u => u.Vendedor)
                .FirstOrDefault(u => u.Email == email);

            if (user?.Comprador == null)
                return Json(new { ok = false, msg = "Apenas compradores podem reservar." });

            var anuncio = _db.Anuncios.FirstOrDefault(a => a.IdAnuncio == idAnuncio);
            if (anuncio == null)
                return Json(new { ok = false, msg = "Anúncio não encontrado." });

            if (anuncio.Estado != "ativo")
                return Json(new { ok = false, msg = "Este veículo não pode ser reservado." });

            if (user.Vendedor != null && user.Vendedor.IdVendedor == anuncio.IdVendedor)
                return Json(new { ok = false, msg = "Não podes reservar o teu próprio veículo." });

            // bloquear se já existir reserva ativa 
            bool existeReservaAtiva = _db.Reservas.Any(r =>
                r.IdAnuncio == idAnuncio &&
                (r.Estado == "confirmada" || r.Estado == "ativada") &&
                r.ExpiraEm > DateTime.Now);

            if (existeReservaAtiva)
                return Json(new { ok = false, msg = "Este veículo encontra-se atualmente reservado." });

            // bloquear se já estiver vendido
            //bool jaVendido = _db.Compras.Any(c =>
            //    c.IdAnuncio == idAnuncio &&
            //    c.EstadoPagamento == "pago");

            //if (jaVendido)
            //    return Json(new { ok = false, msg = "Este veículo já foi vendido." });

            // impedir reserva duplicada pelo mesmo comprador para o mesmo anúncio
            bool jaTemReserva = _db.Reservas.Any(r =>
                r.IdAnuncio == idAnuncio &&
                r.IdComprador == user.Comprador.IdComprador &&
                r.Estado != "cancelada" &&
                r.Estado != "expirada");

            if (jaTemReserva)
                return Json(new { ok = false, msg = "Já fizeste um pedido de reserva para este veículo." });

            var reserva = new Reserva
            {
                IdAnuncio = idAnuncio,
                IdComprador = user.Comprador.IdComprador,
                DataReserva = DateTime.Now,
                ExpiraEm = DateTime.Now.AddHours(horas),
                Estado = "pendente"
            };
            
            _db.Reservas.Add(reserva);
            _db.SaveChanges();

            return Json(new
            {
                ok = true,
                msg = "Pedido de reserva enviado. A aguardar confirmação do vendedor."
            });
        }
    }
}

using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AutoHubProjeto.Controllers
{
    public class VisitasController : Controller
    {
        private readonly ApplicationDbContext _db;

        public VisitasController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ================================================================
        //  DEVOLVE HORAS OCUPADAS PARA UM ANÚNCIO NUMA DATA
        // ================================================================
        [HttpGet]
        public IActionResult HorasOcupadas(int idAnuncio, string data)
        {
            if (!DateTime.TryParse(data, out DateTime parsedDate))
                return Json(new string[] { });

            // pega todas as visitas do dia
            var horas = _db.Visita
                .Where(v => v.IdAnuncio == idAnuncio &&
                            v.DataHora.Date == parsedDate.Date)
                .Select(v => v.DataHora.ToString("HH:mm"))
                .ToList();

            return Json(horas);
        }

        // ================================================================
        //  AGENDAR VISITA
        // ================================================================
        [HttpPost]
        public IActionResult Agendar([FromBody] JsonElement dados)
        {
            int idAnuncio = dados.GetProperty("idAnuncio").GetInt32();
            string dataStr = dados.GetProperty("data").GetString()!;
            string horaStr = dados.GetProperty("hora").GetString()!;

            // validar data
            if (!DateTime.TryParse(dataStr, out DateTime data))
                return Json(new { ok = false, msg = "Data inválida." });

            // validar hora
            if (!TimeSpan.TryParse(horaStr, out TimeSpan hora))
                return Json(new { ok = false, msg = "Hora inválida." });

            // verificar se está autenticado
            if (!User.Identity!.IsAuthenticated)
                return Json(new { ok = false, msg = "Precisas de iniciar sessão." });

            var email = User.Identity!.Name;
            var user = _db.Utilizadors
                .Include(u => u.Comprador)
                .Include(u => u.Vendedor)
                .FirstOrDefault(u => u.Email == email);

            if (user == null || user.Comprador == null)
                return Json(new { ok = false, msg = "Apenas compradores podem agendar visitas." });

            int compradorId = user.Comprador.IdComprador;

            // buscar anuncio
            var anuncio = _db.Anuncios
                .Include(a => a.IdVendedorNavigation)
                .Include(a => a.Visita)
                .FirstOrDefault(a => a.IdAnuncio == idAnuncio);

            if (anuncio == null)
                return Json(new { ok = false, msg = "Anúncio não encontrado." });

            // estados inválidos
            switch (anuncio.Estado)
            {
                case "vendido":
                    return Json(new { ok = false, msg = "O veículo já foi vendido." });
                case "pausado":
                    return Json(new { ok = false, msg = "O anúncio está pausado." });
                case "reservado":
                    return Json(new { ok = false, msg = "O veículo está reservado." });
            }

            // evitar visita ao próprio carro
            if (user.Vendedor != null && user.Vendedor.IdVendedor == anuncio.IdVendedor)
                return Json(new { ok = false, msg = "Não podes agendar visita ao teu próprio veículo." });

            // juntar data + hora
            var dataHora = data.Date.Add(hora);

            if (dataHora <= DateTime.Now)
                return Json(new { ok = false, msg = "A data/hora deve ser futura." });

            // verificar ocupação -> inclui marcadas e pendentes
            bool ocupada = _db.Visita.Any(v =>
                v.IdAnuncio == idAnuncio &&
                v.DataHora == dataHora);

            if (ocupada)
                return Json(new { ok = false, msg = "Esse horário já está ocupado." });

            // criar visita pendente
            var visita = new Visitum
            {
                IdComprador = compradorId,
                IdAnuncio = idAnuncio,
                DataHora = dataHora,
                Estado = "marcada" 
            };

            _db.Visita.Add(visita);
            _db.SaveChanges();

            return Json(new
            {
                ok = true,
                msg = $"Pedido de visita enviado para {dataHora:dd/MM/yyyy HH:mm}."
            });
        }
    }
}

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
                .Where(v =>
                    v.IdAnuncio == idAnuncio &&
                    v.DataHora.Date == parsedDate.Date &&
                    v.Estado == "confirmada"
                )
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

            //  
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
                v.DataHora == dataHora &&
                v.Estado == "confirmada"
            );


            if (ocupada)
                return Json(new { ok = false, msg = "Esse horário já está ocupado." });

            // criar visita pendente
            var visita = new Visitum
            {
                IdComprador = compradorId,
                IdAnuncio = idAnuncio,
                DataHora = dataHora,
                Estado = "pendente" 
            };

            _db.Visita.Add(visita);
            _db.SaveChanges();

            return Json(new
            {
                ok = true,
                msg = $"Pedido de visita enviado para {dataHora:dd/MM/yyyy HH:mm}."
            });
        }

        // ================================================================
        //  VISITAS PENDENTES PARA O VENDEDOR
        // ================================================================
        [HttpGet]
        public IActionResult PendentesVendedor()
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

            var visitas = _db.Visita
                .Include(v => v.IdAnuncioNavigation)
                .Include(v => v.IdCompradorNavigation)
                    .ThenInclude(c => c.IdCompradorNavigation)
                .Where(v =>
                    v.IdAnuncioNavigation.IdVendedor == vendedor.IdVendedor &&
                    v.Estado == "pendente")
                .OrderBy(v => v.DataHora)
                .Select(v => new
                {
                    v.IdVisita,
                    v.DataHora,
                    Anuncio = v.IdAnuncioNavigation.Titulo,
                    Comprador = v.IdCompradorNavigation.IdCompradorNavigation.Nome,
                    Email = v.IdCompradorNavigation.IdCompradorNavigation.Email,
                    Telefone = v.IdCompradorNavigation.IdCompradorNavigation.Telefone
                })
                .ToList();

            return Json(visitas);
        }

        // ================================================================
        //  CONFIRMAR VISITA (VENDEDOR)
        // ================================================================
        [HttpPost]
        public IActionResult Confirmar(int id)
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

            var visita = _db.Visita
                .Include(v => v.IdAnuncioNavigation)
                .FirstOrDefault(v => v.IdVisita == id);

            if (visita == null)
                return Json(new { ok = false, msg = "Visita não encontrada." });

            if (visita.IdAnuncioNavigation.IdVendedor != vendedor.IdVendedor)
                return Unauthorized();

            visita.Estado = "confirmada";
            var conflitos = _db.Visita.Where(v =>
                v.IdAnuncio == visita.IdAnuncio &&
                v.DataHora == visita.DataHora &&
                v.IdVisita != visita.IdVisita &&
                v.Estado == "pendente"
            );

            foreach (var v in conflitos)
            {
                v.Estado = "cancelada";
            }

            _db.SaveChanges();

            return Json(new { ok = true, msg = "Visita confirmada." });
        }

        // ================================================================
        //  RECUSAR VISITA (VENDEDOR)
        // ================================================================
        [HttpPost]
        public IActionResult Recusar(int id)
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

            var visita = _db.Visita
                .Include(v => v.IdAnuncioNavigation)
                .FirstOrDefault(v => v.IdVisita == id);

            if (visita == null)
                return Json(new { ok = false, msg = "Visita não encontrada." });

            if (visita.IdAnuncioNavigation.IdVendedor != vendedor.IdVendedor)
                return Unauthorized();

            visita.Estado = "cancelada";
            _db.SaveChanges();

            return Json(new { ok = true, msg = "Visita cancelada." });
        }

        // ================================================================
        //  MARCAR VISITA COMO REALIZADA (VENDEDOR)
        // ================================================================
        [HttpPost]
        public IActionResult MarcarRealizada(int id)
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

            var visita = _db.Visita
                .Include(v => v.IdAnuncioNavigation)
                .FirstOrDefault(v => v.IdVisita == id);

            if (visita == null)
                return Json(new { ok = false, msg = "Visita não encontrada." });

            if (visita.IdAnuncioNavigation.IdVendedor != vendedor.IdVendedor)
                return Unauthorized();

            if (visita.Estado != "confirmada")
                return Json(new { ok = false, msg = "Só visitas confirmadas podem ser concluídas." });

            visita.Estado = "realizada";
            _db.SaveChanges();

            return Json(new { ok = true, msg = "Visita marcada como realizada." });
        }


    }
}

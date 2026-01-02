using AutoHubProjeto.Helpers; 
using AutoHubProjeto.Models;
using AutoHubProjeto.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace AutoHubProjeto.Controllers
{
    public class PainelController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PainelController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            await ReservaHelper.ExpirarReservasAsync(_db);

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.Identity.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                .Include(u => u.Vendedor)
                .Include(u => u.Administrador)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return RedirectToAction("Login", "Auth");


            int idComprador = user.Comprador?.IdComprador ?? 0;
            int idVendedor = user.Vendedor?.IdVendedor ?? 0;

            var vm = new PainelDashboardVM
            {
                Nome = email,
                IsComprador = user.Comprador != null,
                IsVendedor = user.Vendedor != null,
                IsVendedorAprovado = user.Vendedor != null && user.Vendedor.Aprovado,
                IsAdmin = user.Administrador != null,

                FavoritosCount = idComprador == 0 ? 0 :
                    await _db.Favoritos.CountAsync(f => f.IdComprador == idComprador),

                VisitasCount = idComprador == 0 ? 0 :
                    await _db.Visita.CountAsync(v =>
                        v.IdComprador == idComprador &&
                        v.Estado == "confirmada" &&
                        v.DataHora > DateTime.Now),

                ReservasCount = idComprador == 0 ? 0 :
                    await _db.Reservas.CountAsync(r =>
                        r.IdComprador == idComprador &&
                        (r.Estado == "confirmada" || r.Estado == "ativa") &&
                        r.ExpiraEm > DateTime.Now),

                ComprasCount = idComprador == 0 ? 0 :
                    await _db.Compras.CountAsync(c =>
                        c.IdComprador == idComprador &&
                        c.EstadoPagamento == "pago"),

                AnunciosCount = idVendedor == 0 ? 0 :
                    await _db.Anuncios.CountAsync(a =>
                    a.IdVendedor == idVendedor &&
                    a.Estado == "ativo")
            };

            var atividade = new List<PainelAtividadeVM>();
            var limite = DateTime.Now.AddDays(-30);

            // FAVORITOS
            atividade.AddRange(
                await _db.Favoritos
                    .Where(f => f.IdComprador == idComprador &&
                                f.DataFavorito >= limite)
                    .Include(f => f.Anuncio)
                    .Select(f => new PainelAtividadeVM
                    {
                        Tipo = "Favorito",
                        Titulo = f.Anuncio.Titulo,
                        Data = f.DataFavorito
                    })
                    .ToListAsync()
            );

            // RESERVAS
            atividade.AddRange(
                await _db.Reservas
                    .Where(r =>
                        r.IdComprador == idComprador &&
                        (r.Estado == "ativa" || r.Estado == "expirada") &&
                        r.DataReserva >= limite
                    )
                    .Include(r => r.IdAnuncioNavigation)
                    .Select(r => new PainelAtividadeVM
                    {
                        Tipo = "Reserva",
                        Titulo = r.IdAnuncioNavigation.Titulo,
                        Data = r.DataReserva
                    })
                    .ToListAsync()
            );


            // VISITAS
            atividade.AddRange(
                await _db.Visita
                    .Where(v =>
                        v.IdComprador == idComprador &&
                        (v.Estado == "confirmada" || v.Estado == "realizada") &&
                        v.DataHora >= limite
                    )
                    .Include(v => v.IdAnuncioNavigation)
                    .Select(v => new PainelAtividadeVM
                    {
                        Tipo = "Visita",
                        Titulo = v.IdAnuncioNavigation.Titulo,
                        Data = v.DataHora
                    })
                    .ToListAsync()
            );

            // COMPRAS
            atividade.AddRange(
                await _db.Compras
                    .Where(c =>
                        c.IdComprador == idComprador &&
                        c.EstadoPagamento == "pago" &&
                        c.DataCompra >= limite
                    )
                    .Include(c => c.IdAnuncioNavigation)
                    .Select(c => new PainelAtividadeVM
                    {
                        Tipo = "Compra",
                        Titulo = c.IdAnuncioNavigation.Titulo,
                        Data = c.DataCompra
                    })
                    .ToListAsync()
            );

            vm.Atividade = atividade
                .OrderByDescending(a => a.Data)
                .Take(6)
                .ToList();

            return View(vm);
        }
        public async Task<IActionResult> MinhasVisitas()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.Identity.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Comprador == null)
                return RedirectToAction("Index");

            int id = user.Comprador.IdComprador;
            var agora = DateTime.Now;

            var visitas = await _db.Visita
                .Where(v => v.IdComprador == id)
                .Include(v => v.IdAnuncioNavigation)
                .Include(v => v.IdAnuncioNavigation.AnuncioImagems)
                .OrderBy(v => v.DataHora)
                .ToListAsync();

            var vm = new MinhasVisitasVM();

            foreach (var v in visitas)
            {
                string estadoTexto = v.Estado switch
                {
                    "pendente" when v.DataHora <= agora => "Cancelada",
                    "confirmada" when v.DataHora <= agora => "Concluída",
                    "pendente" => "Pendente",
                    "confirmada" => "Confirmada",
                    "cancelada" => "Cancelada",
                    "realizada" => "Concluída",
                    _ => "Indefinido"
                };


                var item = new MinhasVisitasItemVM
                {
                    IdVisita = v.IdVisita,
                    Titulo = v.IdAnuncioNavigation.Titulo,
                    Imagem = v.IdAnuncioNavigation.AnuncioImagems.FirstOrDefault()?.Url ?? "imgs/carros.jpg",
                    DataHora = v.DataHora,
                    Estado = estadoTexto
                };

                if (
                    (v.Estado == "pendente" || v.Estado == "confirmada") &&
                    v.DataHora > agora
                )
                {
                    vm.Futuras.Add(item);
                }
                else
                {
                    vm.Passadas.Add(item);
                }

            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult Cancelar([FromBody] JsonElement dados)
        {
            int idVisita = dados.GetProperty("idVisita").GetInt32();

            var visita = _db.Visita.FirstOrDefault(v => v.IdVisita == idVisita);

            if (visita == null)
                return Json(new { ok = false, msg = "Visita não encontrada." });

            visita.Estado = "cancelada";
            _db.SaveChanges();

            return Json(new { ok = true, msg = "Visita cancelada com sucesso." });
        }
        public async Task<IActionResult> MinhasReservas()
        {
            await ReservaHelper.ExpirarReservasAsync(_db);

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.Identity.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Comprador == null)
                return RedirectToAction("Index");

            int idComprador = user.Comprador.IdComprador;
            var agora = DateTime.Now;

            var reservas = await _db.Reservas
                .Where(r => r.IdComprador == idComprador)
                .Include(r => r.IdAnuncioNavigation)
                .Include(r => r.IdAnuncioNavigation.AnuncioImagems)
                .OrderByDescending(r => r.DataReserva)
                .ToListAsync();

            var vm = new MinhasReservasVM();

            foreach (var r in reservas)
            {
                string estadoUI = r.Estado switch
                {
                    "pendente" => "Pendente",
                    "ativa" => r.ExpiraEm > agora ? "Ativa" : "Expirada",
                    "cancelada" => "Cancelada",
                    "expirada" => "Expirada",
                    _ => "Indefinido"
                };


                var item = new MinhasReservasItemVM
                {
                    IdReserva = r.IdReserva,
                    Titulo = r.IdAnuncioNavigation.Titulo,
                    Imagem = r.IdAnuncioNavigation.AnuncioImagems.FirstOrDefault()?.Url
                             ?? "imgs/carros.jpg",
                    Estado = estadoUI,
                    DataReserva = r.DataReserva,
                    ExpiraEm = r.ExpiraEm
                };

                if (estadoUI == "Ativa" || estadoUI == "Pendente")
                    vm.AtivasEPendentes.Add(item);
                else
                    vm.ExpiradasECanceladas.Add(item);

            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult CancelarReserva([FromBody] JsonElement dados)
        {
            int idReserva = dados.GetProperty("idReserva").GetInt32();

            var reserva = _db.Reservas.FirstOrDefault(r => r.IdReserva == idReserva);

            if (reserva == null)
                return Json(new { ok = false, msg = "Reserva não encontrada." });

            if (reserva.Estado == "cancelada")
                return Json(new { ok = false, msg = "Reserva já cancelada." });

            reserva.Estado = "cancelada";
            _db.SaveChanges();

            return Json(new { ok = true, msg = "Reserva cancelada com sucesso." });
        }

        public async Task<IActionResult> MinhasCompras()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.Identity.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Comprador == null)
                return RedirectToAction("Index");

            int idComprador = user.Comprador.IdComprador;

            var compras = await _db.Compras
                .Where(c => c.IdComprador == idComprador)
                .Include(c => c.IdAnuncioNavigation)
                .Include(c => c.IdAnuncioNavigation.AnuncioImagems)
                .OrderByDescending(c => c.DataCompra)
                .ToListAsync();

            var vm = new MinhasComprasVM();

            foreach (var c in compras)
            {
                var item = new MinhasComprasItemVM
                {
                    IdCompra = c.IdCompra,
                    Titulo = c.IdAnuncioNavigation.Titulo,
                    Imagem = c.IdAnuncioNavigation.AnuncioImagems.FirstOrDefault()?.Url
                             ?? "imgs/carros.jpg",
                    Valor = c.Valor,
                    DataCompra = c.DataCompra,
                    MetodoPagamento = c.ReferenciaPagamento ?? "Não especificado",
                    Estado = c.EstadoPagamento switch
                    {
                        "pendente" => "Pendente",
                        "pago" => "Confirmada",
                        _ => "Cancelada"
                    }
                };

                if (c.EstadoPagamento == "pendente")
                    vm.Pendentes.Add(item);
                else
                    vm.Historico.Add(item);
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult CancelarCompra([FromBody] JsonElement dados)
        {
            int idCompra = dados.GetProperty("idCompra").GetInt32();

            var compra = _db.Compras.FirstOrDefault(c => c.IdCompra == idCompra);

            if (compra == null)
                return Json(new { ok = false, msg = "Compra não encontrada." });

            if (compra.EstadoPagamento != "pendente")
                return Json(new { ok = false, msg = "Esta compra já não pode ser cancelada." });

            compra.EstadoPagamento = "cancelado";
            _db.SaveChanges();

            return Json(new { ok = true, msg = "Compra cancelada com sucesso." });
        }

        public async Task<IActionResult> Favoritos()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.Identity.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Comprador == null)
                return RedirectToAction("Index");

            int idComprador = user.Comprador.IdComprador;

            var favoritos = await _db.Favoritos
                .Where(f => f.IdComprador == idComprador)
                .Include(f => f.Anuncio)
                .Include(f => f.Anuncio.AnuncioImagems)
                .OrderByDescending(f => f.DataFavorito)
                .ToListAsync();

            var vm = new FavoritosVM();

            foreach (var f in favoritos)
            {
                var anuncio = f.Anuncio;

                vm.Lista.Add(new FavoritoItemVM
                {
                    Id = f.Id,
                    IdAnuncio = anuncio.IdAnuncio,
                    Titulo = anuncio.Titulo,
                    Preco = anuncio.Preco,
                    Imagem = anuncio.AnuncioImagems.FirstOrDefault()?.Url
                             ?? "imgs/carros.jpg",
                    Disponivel = anuncio.Estado == "ativo"
                });
            }

            return View(vm);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult RemoverFavorito([FromBody] JsonElement dados)
        {
            int id = dados.GetProperty("idFavorito").GetInt32();

            var fav = _db.Favoritos.FirstOrDefault(f => f.Id == id);

            if (fav == null)
                return Json(new { ok = false, msg = "Favorito não encontrado." });

            _db.Favoritos.Remove(fav);
            _db.SaveChanges();

            return Json(new { ok = true, msg = "Favorito removido." });
        }

        public async Task<IActionResult> FiltrosGuardados()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.Identity.Name;

            var comprador = await _db.Utilizadors
                .Include(u => u.Comprador)
                .ThenInclude(c => c.FiltroFavoritos)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (comprador?.Comprador == null)
                return RedirectToAction("Index");

            var vm = new FiltrosGuardadosVM();

            foreach (var f in comprador.Comprador.FiltroFavoritos
                         .OrderByDescending(f => f.DataCriacao))
            {
                // gerar descrição 
                var dict = System.Text.Json.JsonSerializer
                    .Deserialize<Dictionary<string, string>>(f.FiltrosJson)
                    ?? new();

                var descricao = string.Join(" · ",
                    dict.Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                        .Select(kv => $"{kv.Key}: {kv.Value}")
                );

                vm.Filtros.Add(new FiltroGuardadoItemVM
                {
                    IdFiltro = f.IdFiltro,
                    Nome = f.Nome,
                    Descricao = descricao
                });
            }

            return View(vm);
        }

        public async Task<IActionResult> MarcasFavoritas()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.Identity.Name;

            var utilizador = await _db.Utilizadors
            .Include(u => u.Comprador)
            .FirstOrDefaultAsync(u => u.Email == email);

            var comprador = utilizador?.Comprador;


            if (comprador == null)
                return RedirectToAction("Index");

            var marcasDisponiveis = await _db.Veiculos
                .Select(v => v.Marca)
                .Where(m => m != null && m != "")
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            var marcasSelecionadas = (comprador.MarcaFavorita ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(m => m.Trim())
                .ToList();

            var vm = new MarcasFavoritasVM
            {
                NotificacoesAtivas = comprador.NotificacoesAtivas
            };

            foreach (var m in marcasDisponiveis)
            {
                vm.Marcas.Add(new MarcaItemVM
                {
                    Nome = m,
                    Selecionada = marcasSelecionadas.Contains(m)
                });
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarMarcas(
            List<string> marcas,
            bool notificacoes)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var email = User.Identity.Name;

            var utilizador = await _db.Utilizadors
            .Include(u => u.Comprador)
            .FirstOrDefaultAsync(u => u.Email == email);

            var comprador = utilizador?.Comprador;


            if (comprador == null)
                return RedirectToAction("Index");

            comprador.MarcaFavorita = marcas != null && marcas.Any()
                ? string.Join(",", marcas)
                : null;

            comprador.NotificacoesAtivas = notificacoes;

            await _db.SaveChangesAsync();

            return RedirectToAction("MarcasFavoritas");
        }

        [HttpGet]
        public async Task<IActionResult> Conta()
        {
            var email = User.Identity!.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                .Include(u => u.Vendedor)
                .Include(u => u.Administrador)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return Unauthorized();

            var vm = new DefinicoesContaVM
            {
                // base
                Nome = user.Nome,
                Username = user.Username,
                Email = user.Email,
                Telefone = user.Telefone,
                Morada = user.Morada,

                // roles
                IsComprador = true,
                IsVendedor = user.Vendedor != null,
                IsAdmin = user.Administrador != null,

                // vendedor
                Nif = user.Vendedor?.Nif,
                TipoVendedor = user.Vendedor?.Tipo,
                DadosFaturacao = user.Vendedor?.DadosFaturacao,
                VendedorAprovado = user.Vendedor?.Aprovado ?? false,
                DataAprovacao = user.Vendedor?.DataAprovacao,

                // admin
                //PodeEntrarModoAdmin = user.Administrador != null
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarNotificacoes(bool notificacoes)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            var email = User.Identity.Name;

            var comprador = await _db.Utilizadors
                .Include(u => u.Comprador)
                .Where(u => u.Email == email)
                .Select(u => u.Comprador)
                .FirstOrDefaultAsync();

            if (comprador == null)
                return Unauthorized();

            comprador.NotificacoesAtivas = notificacoes;
            await _db.SaveChangesAsync();

            return RedirectToAction("Conta");
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarDados(DefinicoesContaVM vm)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            var email = User.Identity.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Vendedor)
                .FirstOrDefaultAsync(u => u.Email == email);


            if (user == null)
                return Unauthorized();

            // ======================
            // VALIDAR CAMPOS VAZIOS
            // ======================

            if (string.IsNullOrWhiteSpace(vm.Nome) ||
                string.IsNullOrWhiteSpace(vm.Username) ||
                string.IsNullOrWhiteSpace(vm.Morada))
            {
                TempData["Toast"] = "Nome, username e morada são obrigatórios.";
                return RedirectToAction("Conta");
            }

            // ======================
            // VALIDAR TELEFONE
            // ======================

            if (string.IsNullOrWhiteSpace(vm.Telefone) || vm.Telefone.Length != 9)
            {
                TempData["Toast"] = "O número de telefone deve ter 9 dígitos.";
                return RedirectToAction("Conta");
            }

            // ======================
            // USERNAME DUPLICADO
            // ======================

            bool usernameExiste = await _db.Utilizadors
                .AnyAsync(u => u.Username == vm.Username && u.Id != user.Id);

            if (usernameExiste)
            {
                TempData["Toast"] = "Este username já está a ser utilizado.";
                return RedirectToAction("Conta");
            }

            // ======================
            // TELEFONE DUPLICADO
            // ======================

            bool telefoneExiste = await _db.Utilizadors
                .AnyAsync(u => u.Telefone == vm.Telefone && u.Id != user.Id);

            if (telefoneExiste)
            {
                TempData["Toast"] = "Este número de telefone já está associado a outra conta.";
                return RedirectToAction("Conta");
            }

            // ======================
            // GUARDAR ALTERAÇÕES
            // ======================

            user.Nome = vm.Nome.Trim();
            user.Username = vm.Username.Trim();
            user.Telefone = vm.Telefone;
            user.Morada = vm.Morada.Trim();

            // ======================
            // DADOS DE VENDEDOR
            // ======================

            if (user.Vendedor != null && user.Vendedor.Aprovado)
            {
                // se vier preenchido, guarda
                if (!string.IsNullOrWhiteSpace(vm.Nif))
                    user.Vendedor.Nif = vm.Nif.Trim();

                user.Vendedor.DadosFaturacao = vm.DadosFaturacao?.Trim();
            }


            await _db.SaveChangesAsync();

            TempData["Toast"] = "Dados atualizados com sucesso.";
            return RedirectToAction("Conta");
        }

        [HttpPost]
        public async Task<IActionResult> Tornar(string Tipo)
        {
            var email = User.Identity!.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Vendedor)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return Unauthorized();

            if (user.Vendedor != null)
            {
                TempData["Toast"] = "Já existe um pedido de vendedor.";
                return RedirectToAction("Index");
            }

            var vendedor = new Vendedor
            {
                IdVendedor = user.Id,
                Tipo = Tipo,
                Nif = null,
                DadosFaturacao = null,
                Aprovado = false
            };

            _db.Vendedors.Add(vendedor);
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Pedido enviado. Aguarda aprovação do administrador.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AlterarPassword(DefinicoesContaVM vm)
        {
            var email = User.Identity!.Name;
            var user = await _db.Utilizadors.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return Unauthorized();

            // Password atual TEM de estar certa
            if (!PasswordHelper.VerifyPassword(vm.PasswordAtual, user.PasswordHash))
            {
                TempData["PwdError"] = "Password atual incorreta.";
                TempData["OpenPwdModal"] = true;
                return RedirectToAction("Conta");
            }

            // Nova ≠ Atual
            if (PasswordHelper.VerifyPassword(vm.NovaPassword, user.PasswordHash))
            {
                TempData["OpenPwdModal"] = true;
                return RedirectToAction("Conta");
            }

            // Nova == Confirmar
            if (vm.NovaPassword != vm.ConfirmarPassword)
            {
                TempData["OpenPwdModal"] = true;
                return RedirectToAction("Conta");
            }

            user.PasswordHash = PasswordHelper.HashPassword(vm.NovaPassword);
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Palavra-passe alterada com sucesso.";
            return RedirectToAction("Conta");
        }

        [HttpGet]
        public async Task<IActionResult> Comprador()
        {
            await ReservaHelper.ExpirarReservasAsync(_db);

            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            var email = User.Identity.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Comprador)
                    .ThenInclude(c => c.Visita)
                .Include(u => u.Comprador)
                    .ThenInclude(c => c.Reservas)
                        .ThenInclude(r => r.IdAnuncioNavigation)
                .Include(u => u.Comprador)
                    .ThenInclude(c => c.FiltroFavoritos)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Comprador == null)
                return Unauthorized();

            var agora = DateTime.Now;
            var vm = new PainelCompradorVM();

            /* ======================
               VISITAS ATIVAS
            ======================= */

            var visitasAtivas = user.Comprador.Visita
                .Where(v =>
                    (v.Estado == "pendente" || v.Estado == "confirmada") &&
                    v.DataHora > agora)
                .ToList();

            vm.NumVisitasAtivas = visitasAtivas.Count;

            foreach (var v in visitasAtivas)
            {
                vm.Acoes.Add(new AcaoCompradorVM
                {
                    Tipo = "Visita",
                    Titulo = v.IdAnuncioNavigation.Titulo,
                    Url = "/Painel/MinhasVisitas"
                });
            }

            /* ======================
               RESERVAS PENDENTES
            ======================= */

            var reservasPendentes = user.Comprador.Reservas
                .Where(r => r.Estado == "pendente")
                .ToList();

            vm.NumReservasPendentes = reservasPendentes.Count;

            foreach (var r in reservasPendentes)
            {
                vm.Acoes.Add(new AcaoCompradorVM
                {
                    Tipo = "Reserva",
                    Titulo = r.IdAnuncioNavigation.Titulo,
                    Url = "/Painel/MinhasReservas"
                });
            }

            /* ======================
               RESERVAS ATIVAS
            ======================= */

            var reservasAtivas = user.Comprador.Reservas
                .Where(r =>
                    r.Estado == "ativa" &&
                    r.ExpiraEm > agora)
                .ToList();

            vm.NumReservasAtivas = reservasAtivas.Count;

            foreach (var r in reservasAtivas)
            {
                vm.Acoes.Add(new AcaoCompradorVM
                {
                    Tipo = "Reserva Ativa",
                    Titulo = r.IdAnuncioNavigation.Titulo,
                    Url = "/Painel/MinhasReservas"
                });
            }

            /* ======================
               COMPRAS
            ======================= */

            vm.NumCompras = await _db.Compras.CountAsync(c =>
                c.IdComprador == user.Comprador.IdComprador &&
                c.EstadoPagamento == "pago"
            );

            /* ======================
               ESTADO GERAL
            ======================= */

            vm.TemAtividade =
                vm.NumVisitasAtivas > 0 ||
                vm.NumReservasPendentes > 0 ||
                vm.NumReservasAtivas > 0 ||
                vm.NumCompras > 0;

            /* ======================
               FAVORITOS (máx 3)
            ======================= */

            vm.Favoritos = await _db.Favoritos
                .Where(f => f.IdComprador == user.Comprador.IdComprador)
                .Include(f => f.Anuncio)
                    .ThenInclude(a => a.AnuncioImagems)
                .OrderByDescending(f => f.DataFavorito)
                .Take(3)
                .Select(f => new FavoritoMiniVM
                {
                    IdAnuncio = f.IdAnuncio,
                    Titulo = f.Anuncio.Titulo,
                    Preco = f.Anuncio.Preco,
                    Imagem = f.Anuncio.AnuncioImagems
                        .OrderBy(i => i.Ordem)
                        .Select(i => "/" + i.Url.TrimStart('/'))
                        .FirstOrDefault()
                })
                .ToListAsync();

            return View(vm);
        }

        public async Task<IActionResult> MeusAnuncios()
        {
            await ReservaHelper.ExpirarReservasAsync(_db);

            var email = User.Identity!.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Vendedor)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Vendedor == null)
                return Unauthorized();

            var anuncios = await _db.Anuncios
                .Include(a => a.AnuncioImagems)
                .Include(a => a.Reservas)
                .Include(a => a.Visita)
                .Include(a => a.Compras)
                .Where(a =>
                    a.IdVendedor == user.Vendedor.IdVendedor &&
                    a.Estado != "removido")
                .OrderByDescending(a => a.DataPublicacao)
                .ToListAsync();

            var vm = new MeusAnunciosVM
            {
                TipoVendedor = user.Vendedor.Tipo,
                Nif = user.Vendedor.Nif,
                DadosFaturacao = user.Vendedor.DadosFaturacao,

                Anuncios = anuncios.Select(a => new AnuncioVM
                {
                    Anuncio = a,

                    // regras
                    TemReserva = a.Reservas.Any(r =>
                        r.Estado == "ativa" && r.ExpiraEm > DateTime.Now),

                    TemVisitas = a.Visita.Any(v =>
                        v.Estado == "pendente" || v.Estado == "confirmada"),

                    TemCompras = a.Compras.Any(c =>
                        c.EstadoPagamento == "pendente")
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Pausar(int id)
        {
            var email = User.Identity!.Name;

            var anuncio = await _db.Anuncios
                .Include(a => a.IdVendedorNavigation)
                .ThenInclude(v => v.IdVendedorNavigation)
                .FirstOrDefaultAsync(a => a.IdAnuncio == id);

            if (anuncio == null)
                return NotFound();

            if (anuncio.IdVendedorNavigation.IdVendedorNavigation.Email != email)
                return Unauthorized();

            if (anuncio.Estado == "ativo")
            {
                anuncio.Estado = "pausado";
                anuncio.DataAtualizacao = DateTime.Now;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MeusAnuncios));
        }

        [HttpPost]
        public async Task<IActionResult> Ativar(int id)
        {
            var email = User.Identity!.Name;

            var anuncio = await _db.Anuncios
                .Include(a => a.IdVendedorNavigation)
                .ThenInclude(v => v.IdVendedorNavigation)
                .FirstOrDefaultAsync(a => a.IdAnuncio == id);

            if (anuncio == null)
                return NotFound();

            if (anuncio.IdVendedorNavigation.IdVendedorNavigation.Email != email)
                return Unauthorized();

            if (anuncio.Estado == "pausado")
            {
                anuncio.Estado = "ativo";
                anuncio.DataAtualizacao = DateTime.Now;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MeusAnuncios));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Remover(int id)
        {
            var email = User.Identity!.Name;

            var anuncio = await _db.Anuncios
                .Include(a => a.IdVendedorNavigation)
                    .ThenInclude(v => v.IdVendedorNavigation)
                .Include(a => a.Reservas)
                .Include(a => a.Visita)
                .Include(a => a.Compras)
                .FirstOrDefaultAsync(a => a.IdAnuncio == id);

            if (anuncio == null)
                return NotFound();

            if (anuncio.IdVendedorNavigation.IdVendedorNavigation.Email != email)
                return Unauthorized();

            // =========================
            // BLOQUEIOS
            // =========================

            // Reserva ATIVA e ainda dentro do tempo
            if (anuncio.Reservas.Any(r =>
                r.Estado == "ativa" &&
                r.ExpiraEm > DateTime.Now))
            {
                TempData["Toast"] =
                    "Este anúncio tem uma reserva ativa. Tens de aguardar o fim do período de reserva.";
                return RedirectToAction(nameof(MeusAnuncios));
            }

            // Visitas pendentes ou confirmadas
            if (anuncio.Visita.Any(v =>
                v.Estado == "pendente" || v.Estado == "confirmada"))
            {
                TempData["Toast"] =
                    "Este anúncio tem visitas agendadas. Cancela as visitas antes de remover.";
                return RedirectToAction(nameof(MeusAnuncios));
            }

            // Compras pendentes
            if (anuncio.Compras.Any(c =>
                c.EstadoPagamento == "pendente"))
            {
                TempData["Toast"] =
                    "Este anúncio tem compras pendentes. Cancela-as antes de remover.";
                return RedirectToAction(nameof(MeusAnuncios));
            }

            // =========================
            // REMOÇÃO 
            // =========================

            anuncio.Estado = "removido";
            anuncio.DataAtualizacao = DateTime.Now;

            await _db.SaveChangesAsync();

            TempData["Toast"] = "Anúncio removido com sucesso.";
            return RedirectToAction(nameof(MeusAnuncios));
        }


        [HttpPost]
            [Authorize]
            public async Task<IActionResult> CriarAnuncio(CriarAnuncioVM vm)
            {
                var email = User.Identity!.Name;

                var user = await _db.Utilizadors
                    .Include(u => u.Vendedor)
                    .FirstOrDefaultAsync(u => u.Email == email);

                // =========================
                // AUTORIZAÇÃO
                // =========================
                if (user?.Vendedor == null || !user.Vendedor.Aprovado)
                    return Unauthorized();

                // =========================
                // REGRA DE NEGÓCIO (EMPRESA)
                // =========================
                if (user.Vendedor.Tipo == "empresa")
                {
                    if (string.IsNullOrWhiteSpace(user.Vendedor.Nif))
                    {
                        TempData["Toast"] =
                            "Para criar anúncios como empresa tens de preencher o NIF nas definições da conta.";
                        return RedirectToAction("Conta");
                    }

                    if (string.IsNullOrWhiteSpace(user.Vendedor.DadosFaturacao))
                    {
                        TempData["Toast"] =
                            "Para criar anúncios como empresa tens de preencher os dados de faturação.";
                        return RedirectToAction("Conta");
                    }
                }

                // =========================
                // IMAGENS (EXATAMENTE 4)
                // =========================
                var files = Request.Form.Files.ToList();

                if (files.Count != 4)
                {
                    TempData["Toast"] = "Tens de selecionar exatamente 4 imagens.";
                    return RedirectToAction("MeusAnuncios");
                }

                // =========================
                // CRIAR VEÍCULO
                // =========================
                var veiculo = new Veiculo
                {
                    Marca = vm.Marca,
                    Modelo = vm.Modelo,
                    Categoria = vm.Categoria,
                    Ano = vm.Ano,
                    Quilometragem = vm.Quilometragem,
                    Combustivel = vm.Combustivel,
                    Caixa = vm.Caixa,
                    Localizacao = vm.Localizacao,
                    Descricao = vm.Descricao
                };

                _db.Veiculos.Add(veiculo);
                await _db.SaveChangesAsync();

                // =========================
                // CRIAR ANÚNCIO
                // =========================
                var anuncio = new Anuncio
                {
                    IdVendedor = user.Vendedor.IdVendedor,
                    IdVeiculo = veiculo.IdVeiculo,
                    Titulo = vm.Titulo,
                    Preco = vm.Preco,
                    Estado = "ativo",
                    DataPublicacao = DateTime.Now
                };

                _db.Anuncios.Add(anuncio);
                await _db.SaveChangesAsync();

                // =========================
                // PASTA DO VEÍCULO
                // =========================
                var pastaVeiculo = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "imgs",
                    "veiculos",
                    veiculo.IdVeiculo.ToString()
                );

                Directory.CreateDirectory(pastaVeiculo);

                // =========================
                // GUARDAR IMAGENS (1.jpg → 4.jpg)
                // =========================
                int ordem = 1;

                foreach (var img in files)
                {
                    var fileName = $"{ordem}.jpg";
                    var path = Path.Combine(pastaVeiculo, fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await img.CopyToAsync(stream);

                    _db.AnuncioImagems.Add(new AnuncioImagem
                    {
                        IdAnuncio = anuncio.IdAnuncio,
                        Url = $"imgs/veiculos/{veiculo.IdVeiculo}/{fileName}",
                        Ordem = ordem
                    });

                    ordem++;
                }

                await _db.SaveChangesAsync();

                TempData["Toast"] = "Anúncio criado com sucesso.";
                return RedirectToAction("MeusAnuncios");
            }

        public async Task<IActionResult> Reservas()
        {
            await ReservaHelper.ExpirarReservasAsync(_db);

            var email = User.Identity!.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Vendedor)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Vendedor == null)
                return Unauthorized();

            int idVendedor = user.Vendedor.IdVendedor;
            var agora = DateTime.Now;

            var reservas = await _db.Reservas
                .Include(r => r.IdAnuncioNavigation)
                    .ThenInclude(a => a.AnuncioImagems)
                .Where(r => r.IdAnuncioNavigation.IdVendedor == idVendedor)
                .OrderByDescending(r => r.DataReserva)
                .ToListAsync();

            var vm = new ReservasClientesVM();

            foreach (var r in reservas)
            {
                string estadoUI = r.Estado switch
                {
                    "pendente" => "Pendente",
                    "ativa" => r.ExpiraEm > agora ? "Ativa" : "Expirada",
                    "cancelada" => "Cancelada",
                    "expirada" => "Expirada",
                    _ => "Indefinido"
                };


                var item = new ReservasClientesItemVM
                {
                    IdReserva = r.IdReserva,
                    IdAnuncio = r.IdAnuncio,
                    Titulo = r.IdAnuncioNavigation.Titulo,
                    Imagem = "/" + (
                        r.IdAnuncioNavigation.AnuncioImagems.FirstOrDefault()?.Url
                        ?? "imgs/placeholder-car.png"
                    ),
                    Estado = estadoUI,
                    DataReserva = r.DataReserva,
                    ExpiraEm = r.ExpiraEm
                };

                if (estadoUI == "Pendente")
                    vm.Pendentes.Add(item);
                else if (estadoUI == "Ativa")
                    vm.Ativas.Add(item);
                else
                    vm.ExpiradasECanceladas.Add(item);

            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult ConfirmarReserva([FromBody] ReservaAcaoVM vm)
        {
            var reserva = _db.Reservas
                .Include(r => r.IdAnuncioNavigation)
                .FirstOrDefault(r => r.IdReserva == vm.Id);

            if (reserva == null || reserva.Estado != "pendente")
                return BadRequest();

            reserva.Estado = "ativa";
            reserva.IdAnuncioNavigation.Estado = "reservado";

            _db.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult CancelarReservaVendedor([FromBody] ReservaAcaoVM vm)
        {
            var reserva = _db.Reservas
                .Include(r => r.IdAnuncioNavigation)
                .FirstOrDefault(r => r.IdReserva == vm.Id);

            if (reserva == null)
                return BadRequest();

            reserva.Estado = "cancelada";
            reserva.IdAnuncioNavigation.Estado = "ativo";

            _db.SaveChanges();
            return Ok();
        }

        public async Task<IActionResult> Visitas()
        {
            var email = User.Identity!.Name;

            var user = await _db.Utilizadors
                .Include(u => u.Vendedor)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Vendedor == null)
                return Unauthorized();

            int idVendedor = user.Vendedor.IdVendedor;
            var agora = DateTime.Now;

            var visitas = await _db.Visita
                .Include(v => v.IdAnuncioNavigation)
                    .ThenInclude(a => a.AnuncioImagems)
                .Where(v => v.IdAnuncioNavigation.IdVendedor == idVendedor)
                .OrderBy(v => v.DataHora)
                .ToListAsync();

            var vm = new VisitasAgendadasVM();

            foreach (var v in visitas)
            {
                string estadoUI = v.Estado switch
                {
                    "pendente" when v.DataHora <= agora => "Cancelada",
                    "confirmada" when v.DataHora <= agora => "Realizada",
                    "pendente" => "Pendente",
                    "confirmada" => "Confirmada",
                    "cancelada" => "Cancelada",
                    "realizada" => "Realizada",
                    _ => "Indefinido"
                };


                var item = new VisitaAgendadaItemVM
                {
                    IdVisita = v.IdVisita,
                    Titulo = v.IdAnuncioNavigation.Titulo,
                    Imagem = "/" + (v.IdAnuncioNavigation.AnuncioImagems.FirstOrDefault()?.Url
                                    ?? "imgs/placeholder-car.png"),
                    DataHora = v.DataHora,
                    Estado = estadoUI
                };

                if (v.Estado == "pendente")
                    vm.Pendentes.Add(item);
                else if (v.Estado == "confirmada" && v.DataHora > agora)
                    vm.Confirmadas.Add(item);
                else
                    vm.CanceladasRealizadas.Add(item);

            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult ConfirmarVisita([FromBody] ReservaAcaoVM vm)
        {
            if (vm == null || vm.Id <= 0)
                return BadRequest();

            var visita = _db.Visita
                .Include(v => v.IdAnuncioNavigation)
                .FirstOrDefault(v => v.IdVisita == vm.Id);

            if (visita == null || visita.Estado != "pendente")
                return BadRequest();

            // confirmar esta visita
            visita.Estado = "confirmada";

            // cancelar outras visitas pendentes
            var visitasConflito = _db.Visita
                .Where(v =>
                    v.IdAnuncio == visita.IdAnuncio &&
                    v.DataHora == visita.DataHora &&
                    v.Estado == "pendente" &&
                    v.IdVisita != visita.IdVisita
                )
                .ToList();

            foreach (var v in visitasConflito)
            {
                v.Estado = "cancelada";
            }

            _db.SaveChanges();
            return Ok();
        }


        [HttpPost]
        public IActionResult CancelarVisitaVendedor([FromBody] ReservaAcaoVM vm)
        {
            if (vm == null || vm.Id <= 0)
                return BadRequest();

            var visita = _db.Visita
                .FirstOrDefault(v => v.IdVisita == vm.Id);

            if (visita == null)
                return BadRequest();

            visita.Estado = "cancelada";

            _db.SaveChanges();
            return Ok();
        }

    }
} 

using System;
using System.Collections.Generic;

namespace AutoHubProjeto.Models;

public partial class Utilizador
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public string? Morada { get; set; }

    public string? Telefone { get; set; }

    public DateTime DataCriacao { get; set; }

    public string EstadoConta { get; set; } = null!;

    public virtual Administrador? Administrador { get; set; }

    public virtual Comprador? Comprador { get; set; }

    public virtual Vendedor? Vendedor { get; set; }
}

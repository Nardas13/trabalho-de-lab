using System;
using System.Collections.Generic;

namespace AutoHubProjeto.Models;

public partial class LogAdministrativo
{
    public long IdLog { get; set; }

    public int IdAdmin { get; set; }

    public DateTime DataHora { get; set; }

    public string Acao { get; set; } = null!;

    public string? Entidade { get; set; }

    public string? IdEntidade { get; set; }

    public string? Detalhes { get; set; }

    public virtual Administrador IdAdminNavigation { get; set; } = null!;
}

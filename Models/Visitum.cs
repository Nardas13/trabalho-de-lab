using System;
using System.Collections.Generic;

namespace AutoHubProjeto.Models;

public partial class Visitum
{
    public int IdVisita { get; set; }

    public int IdComprador { get; set; }

    public int IdAnuncio { get; set; }

    public DateTime DataHora { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;

    public virtual Comprador IdCompradorNavigation { get; set; } = null!;
}

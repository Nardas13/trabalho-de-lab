using System;
using System.Collections.Generic;

namespace AutoHubProjeto.Models;

public partial class AnuncioImagem
{
    public int IdImagem { get; set; }

    public int IdAnuncio { get; set; }

    public string Url { get; set; } = null!;

    public int? Ordem { get; set; }

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace AutoHubProjeto.Models;

public partial class Vendedor
{
    public int IdVendedor { get; set; }

    public string Nif { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string? DadosFaturacao { get; set; }

    public bool Aprovado { get; set; }

    public int? IdAdminAprovador { get; set; }

    public DateTime? DataAprovacao { get; set; }

    public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

    public virtual Administrador? IdAdminAprovadorNavigation { get; set; }

    public virtual Utilizador IdVendedorNavigation { get; set; } = null!;
}

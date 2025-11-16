using System;
using System.Collections.Generic;

namespace AutoHubProjeto.Models;

public partial class Compra
{
    public int IdCompra { get; set; }

    public int IdComprador { get; set; }

    public int IdAnuncio { get; set; }

    public decimal Valor { get; set; }

    public DateTime DataCompra { get; set; }

    public string? ReferenciaPagamento { get; set; }

    public string EstadoPagamento { get; set; } = null!;

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;

    public virtual Comprador IdCompradorNavigation { get; set; } = null!;
}

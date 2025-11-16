using System;
using System.Collections.Generic;

namespace AutoHubProjeto.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public int IdComprador { get; set; }

    public int IdAnuncio { get; set; }

    public DateTime DataReserva { get; set; }

    public DateTime ExpiraEm { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;

    public virtual Comprador IdCompradorNavigation { get; set; } = null!;
}

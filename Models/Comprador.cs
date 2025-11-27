using System;
using System.Collections.Generic;

namespace AutoHubProjeto.Models;

public partial class Comprador
{
    public int IdComprador { get; set; }

    public string? MarcaFavorita { get; set; }

    public bool NotificacoesAtivas { get; set; }

    public string? FiltroFavorito { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual Utilizador IdCompradorNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Visitum> Visita { get; set; } = new List<Visitum>();
}

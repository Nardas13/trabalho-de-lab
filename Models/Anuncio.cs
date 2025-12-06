using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoHubProjeto.Models;

public partial class Anuncio
{
    public int IdAnuncio { get; set; }

    public int IdVendedor { get; set; }

    public int IdVeiculo { get; set; }

    public string Titulo { get; set; } = null!;

    public decimal Preco { get; set; }

    public DateTime DataPublicacao { get; set; }

    public DateTime? DataAtualizacao { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<AnuncioImagem> AnuncioImagems { get; set; } = new List<AnuncioImagem>();

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual Veiculo IdVeiculoNavigation { get; set; } = null!;

    public virtual Vendedor IdVendedorNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Visitum> Visita { get; set; } = new List<Visitum>();
    
    [NotMapped]
    public bool IsFavorito { get; set; }

}

using System;

namespace AutoHubProjeto.Models
{
    public partial class FiltroFavorito
    {
        public int IdFiltro { get; set; }

        public int IdComprador { get; set; }

        public string Nome { get; set; } = null!;

        public string FiltrosJson { get; set; } = null!;

        public DateTime DataCriacao { get; set; }

        public virtual Comprador Comprador { get; set; } = null!;
    }
}

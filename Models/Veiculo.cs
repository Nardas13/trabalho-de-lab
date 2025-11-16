using System;
using System.Collections.Generic;

namespace AutoHubProjeto.Models;

public partial class Veiculo
{
    public int IdVeiculo { get; set; }

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public short Ano { get; set; }

    public decimal? PrecoSugerido { get; set; }

    public int Quilometragem { get; set; }

    public string Combustivel { get; set; } = null!;

    public string Caixa { get; set; } = null!;

    public string? Localizacao { get; set; }

    public string? Descricao { get; set; }

    public virtual Anuncio? Anuncio { get; set; }
}

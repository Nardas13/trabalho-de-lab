using System.Collections.Generic;

namespace AutoHubProjeto.Models
{
    public class FiltroAnunciosViewModel
    {
        // ----- FILTROS -----
        public string Categoria { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }

        public int? AnoMin { get; set; }
        public int? AnoMax { get; set; }

        public decimal? PrecoMin { get; set; }
        public decimal? PrecoMax { get; set; }

        public int? KmMax { get; set; }


        public string Combustivel { get; set; }
        public string Caixa { get; set; }
        public string Localizacao { get; set; }

        public string Ordenar { get; set; }

        // ----- RESULTADOS -----
        public List<Anuncio> Resultados { get; set; }

        // ----- LISTAS DINÂMICAS -----
        public List<string> Marcas { get; set; }
        public List<string> Categorias { get; set; }
        public List<string> Localizacoes { get; set; }
    }
}

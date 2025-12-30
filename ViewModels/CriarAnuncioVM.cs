using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AutoHubProjeto.ViewModels
{
    public class CriarAnuncioVM
    {
        // Anúncio
        public string Titulo { get; set; }
        public decimal Preco { get; set; }

        // Veículo
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Categoria { get; set; }
        public short Ano { get; set; }
        public int Quilometragem { get; set; }
        public string Combustivel { get; set; }
        public string Caixa { get; set; }
        public string? Localizacao { get; set; }
        public string? Descricao { get; set; }

        // Imagens (EXATAMENTE 4)
        public List<IFormFile> Imagens { get; set; } = new();
    }
}

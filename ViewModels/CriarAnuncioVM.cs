using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AutoHubProjeto.ViewModels
{
    public class CriarAnuncioVM
    {
        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [Range(0, 10000000)]
        public decimal Preco { get; set; }

        [Required]
        [StringLength(50)]
        public string Marca { get; set; }

        [Required]
        [StringLength(50)]
        public string Modelo { get; set; }

        [Required]
        [StringLength(30)]
        public string Categoria { get; set; }

        [Required]
        [Range(1900, 2025)]
        public short Ano { get; set; }

        [Required]
        [Range(0, 2_000_000)]
        public int Quilometragem { get; set; }

        [Required]
        public string Combustivel { get; set; }

        [Required]
        public string Caixa { get; set; }

        // opcionais mas validados
        [StringLength(100, MinimumLength = 2)]
        public string? Localizacao { get; set; }

        [StringLength(2000)]
        public string? Descricao { get; set; }

        [Required]
        [MinLength(4), MaxLength(4)]
        public List<IFormFile> Imagens { get; set; } = new();

    }
}

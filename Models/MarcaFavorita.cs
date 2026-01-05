using AutoHubProjeto.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace AutoHubProjeto.Models
{
    public class MarcaFavorita
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Comprador))]
        public int IdComprador { get; set; }

        public string Marca { get; set; } = null!;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public virtual Comprador Comprador { get; set; } = null!;
    }
}
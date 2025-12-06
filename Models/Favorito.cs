using AutoHubProjeto.Models;

public class Favorito
{
    public int Id { get; set; }

    public int IdComprador { get; set; }
    public int IdAnuncio { get; set; }

    public DateTime DataFavorito { get; set; } = DateTime.Now;

    public virtual Comprador Comprador { get; set; }
    public virtual Anuncio Anuncio { get; set; }
}

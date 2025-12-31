using AutoHubProjeto.Models;

public class MeusAnunciosVM
{
    public List<Anuncio> Anuncios { get; set; } = new();

    // dados do vendedor
    public string? TipoVendedor { get; set; }
    public string? Nif { get; set; }
    public string? DadosFaturacao { get; set; }
}

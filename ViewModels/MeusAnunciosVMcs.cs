using AutoHubProjeto.Models;

public class MeusAnunciosVM
{
    public List<AnuncioVM> Anuncios { get; set; } = new();

    // dados do vendedor
    public string? TipoVendedor { get; set; }
    public string? Nif { get; set; }
    public string? DadosFaturacao { get; set; }
}

public class AnuncioVM
{
    public Anuncio Anuncio { get; set; } = null!;

    public bool TemReserva { get; set; }
    public bool TemVisitas { get; set; }
    public bool TemCompras { get; set; }
}

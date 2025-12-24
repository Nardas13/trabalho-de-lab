namespace AutoHubProjeto.ViewModels
{
    public class PainelCompradorVM
    {
        /* ======================
           ESTADO GERAL
        ======================= */

        public bool TemAtividade { get; set; }
        public int NumVisitasAtivas { get; set; }
        public int NumReservasPendentes { get; set; }

        /* ======================
           AÇÕES
        ======================= */

        public List<AcaoCompradorVM> Acoes { get; set; } = new();

        /* ======================
           FAVORITOS
        ======================= */

        public List<FavoritoMiniVM> Favoritos { get; set; } = new();
    }

    public class AcaoCompradorVM
    {
        public string Tipo { get; set; } = null!;   // Visita / Reserva
        public string Titulo { get; set; } = null!;
        public string Url { get; set; } = null!;
    }

    public class FavoritoMiniVM
    {
        public int IdAnuncio { get; set; }
        public string Titulo { get; set; } = null!;
        public string Imagem { get; set; } = null!;
        public decimal Preco { get; set; }
    }
}

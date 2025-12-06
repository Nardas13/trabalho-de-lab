namespace AutoHubProjeto.ViewModels
{
    public class PainelDashboardVM
    {
        public string Nome { get; set; }

        public bool IsComprador { get; set; }
        public bool IsVendedor { get; set; }

        public int FavoritosCount { get; set; }
        public int VisitasCount { get; set; }
        public int ReservasCount { get; set; }
        public int ComprasCount { get; set; }
        public int AnunciosCount { get; set; }

        public List<PainelAtividadeVM> Atividade { get; set; } = new();

    }
}

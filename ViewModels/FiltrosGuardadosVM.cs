namespace AutoHubProjeto.ViewModels
{
    public class FiltroGuardadoItemVM
    {
        public int IdFiltro { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }

    public class FiltrosGuardadosVM
    {
        public List<FiltroGuardadoItemVM> Filtros { get; set; } = new();
    }
}

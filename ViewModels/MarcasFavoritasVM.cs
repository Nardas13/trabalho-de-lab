namespace AutoHubProjeto.ViewModels
{
    public class MarcaItemVM
    {
        public string Nome { get; set; }
        public bool Selecionada { get; set; }
    }

    public class MarcasFavoritasVM
    {
        public List<MarcaItemVM> Marcas { get; set; } = new();
        public bool NotificacoesAtivas { get; set; }
    }
}

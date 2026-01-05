namespace AutoHubProjeto.ViewModels
{

    public class MarcaItemVM
    {
        public string Nome { get; set; } = null!;
        public bool Selecionada { get; set; }
    }

    public class MarcasFavoritasVM
    {
        public List<MarcaItemVM> Marcas { get; set; } = new();
    }
}

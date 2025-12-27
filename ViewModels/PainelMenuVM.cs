namespace AutoHubProjeto.ViewModels
{
    public class PainelMenuVM
    {
        public bool IsComprador { get; set; }
        public bool IsVendedor { get; set; }
        public bool IsVendedorAprovado { get; set; } 

        public bool IsAdmin { get; set; }

        public int FavoritosCount { get; set; }
    }
}

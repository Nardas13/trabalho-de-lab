namespace AutoHubProjeto.ViewModels
{
    public class FavoritoItemVM
    {
        public int Id { get; set; } // = Favorito.Id
        public int IdAnuncio { get; set; }

        public string Titulo { get; set; }
        public string Imagem { get; set; }
        public decimal Preco { get; set; }

        public bool Disponivel { get; set; }
    }

    public class FavoritosVM
    {
        public List<FavoritoItemVM> Lista { get; set; } = new();
    }
}

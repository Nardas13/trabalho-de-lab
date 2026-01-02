namespace AutoHubProjeto.ViewModels
{
    public class VendasItemVM
    {
        public int IdCompra { get; set; }

        public string Titulo { get; set; }
        public string Imagem { get; set; }

        public decimal Valor { get; set; }
        public DateTime DataCompra { get; set; }

        public string CompradorEmail { get; set; } 
        public string Estado { get; set; }
    }

    public class VendasVM
    {
        public List<VendasItemVM> Pendentes { get; set; } = new();
        public List<VendasItemVM> Concluidas { get; set; } = new();
        public List<VendasItemVM> Canceladas { get; set; } = new();
    }

}

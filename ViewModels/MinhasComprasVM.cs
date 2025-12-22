namespace AutoHubProjeto.ViewModels
{
    public class MinhasComprasItemVM
    {
        public int IdCompra { get; set; }

        public string Titulo { get; set; }
        public string Imagem { get; set; }

        public decimal Valor { get; set; }
        public DateTime DataCompra { get; set; }

        public string MetodoPagamento { get; set; }
        public string Estado { get; set; }
    }

    public class MinhasComprasVM
    {
        public List<MinhasComprasItemVM> Pendentes { get; set; } = new();
        public List<MinhasComprasItemVM> Historico { get; set; } = new();
    }
}

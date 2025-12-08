namespace AutoHubProjeto.ViewModels
{
    public class MinhasVisitasItemVM
    {
        public int IdVisita { get; set; }
        public string Titulo { get; set; }
        public string Imagem { get; set; }
        public DateTime DataHora { get; set; }
        public string Estado { get; set; } // Agendada, Cancelada, Concluída
    }

    public class MinhasVisitasVM
    {
        public List<MinhasVisitasItemVM> Futuras { get; set; } = new();
        public List<MinhasVisitasItemVM> Passadas { get; set; } = new();
    }
}

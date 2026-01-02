namespace AutoHubProjeto.ViewModels
{
    public class VisitaAgendadaItemVM
    {
        public int IdVisita { get; set; }
        public string Titulo { get; set; }
        public string Imagem { get; set; }
        public DateTime DataHora { get; set; }
        public string Estado { get; set; }
    }

    public class VisitasAgendadasVM
    {
        public List<VisitaAgendadaItemVM> Pendentes { get; set; } = new();
        public List<VisitaAgendadaItemVM> Confirmadas { get; set; } = new();
        public List<VisitaAgendadaItemVM> CanceladasRealizadas { get; set; } = new();
    }
}

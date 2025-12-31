namespace AutoHubProjeto.ViewModels
{
    public class ReservasClientesItemVM
    {
        public int IdReserva { get; set; }
        public int IdAnuncio { get; set; }

        public string Titulo { get; set; }
        public string Imagem { get; set; }

        public string Estado { get; set; }

        public DateTime DataReserva { get; set; }
        public DateTime? ExpiraEm { get; set; }
    }

    public class ReservasClientesVM
    {
        public List<ReservasClientesItemVM> Pendentes { get; set; } = new();
        public List<ReservasClientesItemVM> Ativas { get; set; } = new();
        public List<ReservasClientesItemVM> ExpiradasECanceladas { get; set; } = new();
    }
}

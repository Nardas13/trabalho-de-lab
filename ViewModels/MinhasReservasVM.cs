namespace AutoHubProjeto.ViewModels
{
    public class MinhasReservasItemVM
    {
        public int IdReserva { get; set; }

        public string Titulo { get; set; }
        public string Imagem { get; set; }

        public string Estado { get; set; }

        // Para PENDENTE
        public DateTime DataReserva { get; set; }

        // Para ATIVA / EXPIRADA
        public DateTime? ExpiraEm { get; set; }
    }

    public class MinhasReservasVM
    {
        public List<MinhasReservasItemVM> AtivasEPendentes { get; set; } = new();
        public List<MinhasReservasItemVM> ExpiradasECanceladas { get; set; } = new();
    }
}

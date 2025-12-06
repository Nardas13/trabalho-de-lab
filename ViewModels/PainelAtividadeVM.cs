namespace AutoHubProjeto.ViewModels
{
    public class PainelAtividadeVM
    {
        public string Tipo { get; set; }   // Favorito, Reserva, Visita, Compra
        public string Titulo { get; set; } // Veículo
        public DateTime Data { get; set; }

        public string TempoDecorrido
        {
            get
            {
                var diff = DateTime.Now - Data;

                if (diff.TotalMinutes < 60)
                    return $"{(int)diff.TotalMinutes} min atrás";

                if (diff.TotalHours < 24)
                    return $"{(int)diff.TotalHours} h atrás";

                if (diff.TotalDays < 7)
                    return $"{(int)diff.TotalDays} dias atrás";

                return Data.ToString("dd MMM");
            }
        }
    }
}

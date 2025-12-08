namespace AutoHubProjeto.ViewModels
{
    public class PainelAtividadeVM
    {
        public string Tipo { get; set; }   // Favorito, Reserva, Visita, Compra
        public string Titulo { get; set; }
        public DateTime Data { get; set; }

        public string TempoDecorrido
        {
            get
            {
                var agora = DateTime.Now;

                // Arredonda data ao minuto
                var dataLocal = DateTime.SpecifyKind(Data, DateTimeKind.Local);
                var dataArred = new DateTime(dataLocal.Year, dataLocal.Month, dataLocal.Day, dataLocal.Hour, dataLocal.Minute, 0);
                var agoraArred = new DateTime(agora.Year, agora.Month, agora.Day, agora.Hour, agora.Minute, 0);

                var diff = agoraArred - dataArred;

                // ----------------------------------------
                // CASO 1 -> EVENTO NO FUTURO (VISITAS)
                // ----------------------------------------
                if (diff.TotalMinutes < 0)
                {
                    diff = dataArred - agoraArred;

                    if (diff.TotalMinutes < 60)
                        return $"em {(int)diff.TotalMinutes} min";

                    if (diff.TotalHours < 24)
                        return $"em {(int)diff.TotalHours} h";

                    if (diff.TotalDays < 2)
                        return "amanhã";

                    return $"em {(int)diff.TotalDays} dias";
                }

                // ----------------------------------------
                // CASO 2 -> EVENTO NO PASSADO
                // ----------------------------------------

                if (diff.TotalMinutes < 1)
                    return "agora mesmo";

                if (diff.TotalMinutes < 60)
                    return $"{(int)diff.TotalMinutes} min atrás";

                if (diff.TotalHours < 24)
                    return $"{(int)diff.TotalHours} h atrás";

                if (diff.TotalDays < 7)
                    return $"{(int)diff.TotalDays} dias atrás";

                if (diff.TotalDays < 30)
                    return $"{(int)(diff.TotalDays / 7)} semana(s) atrás";

                return dataArred.ToString("dd MMM");
            }
        }

    }
}

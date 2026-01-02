using AutoHubProjeto.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.Helpers
{
    public static class ReservaHelper
    {
        public static async Task ExpirarReservasAsync(ApplicationDbContext db)
        {
            var agora = DateTime.Now;

            var reservasExpiradas = await db.Reservas
                .Include(r => r.IdAnuncioNavigation)
                .Where(r =>
                    r.Estado == "ativa" &&
                    r.ExpiraEm <= agora
                )
                .ToListAsync();

            if (!reservasExpiradas.Any())
                return;

            foreach (var r in reservasExpiradas)
            {
                // marcar reserva como expirada
                r.Estado = "expirada";

                // libertar anúncio
                if (r.IdAnuncioNavigation != null &&
                    r.IdAnuncioNavigation.Estado == "reservado")
                {
                    r.IdAnuncioNavigation.Estado = "ativo";
                }
            }

            await db.SaveChangesAsync();
        }
    }
}

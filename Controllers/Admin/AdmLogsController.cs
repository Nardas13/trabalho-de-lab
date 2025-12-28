using AutoHubProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoHubProjeto.Controllers.Admin
{
    public class AdmLogsController : AdmBaseController
    {
        public AdmLogsController(ApplicationDbContext db) : base(db) { }

        public IActionResult Index()
        {
            var logs = _db.LogAdministrativos
                .Include(l => l.IdAdminNavigation)
                    .ThenInclude(a => a.IdAdminNavigation)
                .OrderByDescending(l => l.DataHora)
                .Take(500)
                .ToList();

            return View(logs);
        }
    }
}

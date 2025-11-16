using System;
using System.Collections.Generic;

namespace AutoHubProjeto.Models;

public partial class Administrador
{
    public int IdAdmin { get; set; }

    public virtual Utilizador IdAdminNavigation { get; set; } = null!;

    public virtual ICollection<LogAdministrativo> LogAdministrativos { get; set; } = new List<LogAdministrativo>();

    public virtual ICollection<Vendedor> Vendedors { get; set; } = new List<Vendedor>();
}

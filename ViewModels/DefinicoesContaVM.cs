namespace AutoHubProjeto.ViewModels
{
    public class DefinicoesContaVM
    {
        /* ======================
           DADOS BASE
        ======================= */

        public string Nome { get; set; }
        public string Username { get; set; }  
        public string Email { get; set; }      

        public string Telefone { get; set; }
        public string Morada { get; set; }

        /* ======================
           PASSWORD
        ======================= */

        public string PasswordAtual { get; set; }
        public string NovaPassword { get; set; }
        public string ConfirmarPassword { get; set; }

        /* ======================
           ESTADOS / ROLES
        ======================= */

        public bool IsComprador { get; set; }  
        public bool IsVendedor { get; set; }
        public bool IsAdmin { get; set; }

        /* ======================
           DADOS DE VENDEDOR
        ======================= */

        public string? Nif { get; set; }
        public string? TipoVendedor { get; set; }   // Particular / Empresa
        public string? DadosFaturacao { get; set; }

        public bool VendedorAprovado { get; set; }
        public DateTime? DataAprovacao { get; set; }

        /* ======================
           ADMIN
        ======================= */

        public bool PodeEntrarModoAdmin { get; set; }
    }
}
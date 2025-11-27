namespace AutoHubProjeto.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string html);
        Task SendVerificationEmail(string email, string code);
        Task SendResetPasswordEmail(string email, string code);
    }
}

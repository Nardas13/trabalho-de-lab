using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace AutoHubProjeto.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string html)
        {
            try
            {
                using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                    EnableSsl = _settings.EnableSsl
                };

                using var mail = new MailMessage
                {
                    From = new MailAddress(_settings.FromEmail, _settings.FromName),
                    Subject = subject,
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                var view = AlternateView.CreateAlternateViewFromString(html, null, "text/html");

                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "logotipodourado.jpg");

                if (File.Exists(logoPath))
                {
                    var logo = new LinkedResource(logoPath)
                    {
                        ContentId = "autohub-logo",
                        ContentType = new System.Net.Mime.ContentType("image/jpeg"),
                        TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                    };

                    view.LinkedResources.Add(logo);
                }


                mail.AlternateViews.Add(view);

                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao enviar email: " + ex.Message);
            }
        }

        public async Task SendVerificationEmail(string email, string code)
        {
            var html = EmailTemplates.VerificationCode(code);
            await SendEmailAsync(email, "Código AutoHub – Verificação de Email", html);
        }

        public async Task SendResetPasswordEmail(string email, string code)
        {
            var html = EmailTemplates.ResetPasswordCode(code);
            await SendEmailAsync(email, "Código AutoHub – Recuperar Password", html);
        }
    }
}
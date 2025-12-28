using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.Metrics;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace AutoHubProjeto.Services
{
    public static class EmailTemplates
    {
        public static string VerificationCode(string code)
        {
            return $@"
<!DOCTYPE html>
<html>
<body style='margin:0;padding:0;background:#f5f5f5;font-family:Arial,Helvetica,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' style='padding:40px 0;'>
<tr><td align='center'>

    <table width='600' cellpadding='0' cellspacing='0'
        style='background:#ffffff;border-radius:14px;padding:40px;
               box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

        <!-- TÍTULO + LOGO 100% COMPATÍVEL GMAIL -->
        <tr>
            <td align='center' style='padding-bottom:20px;'>
                <table cellpadding='0' cellspacing='0' style='margin:auto;'>
                    <tr>
                        <td>
                            <img src='cid:autohub-logo' width='44' height='44'
                                 style='vertical-align:middle;margin-right:12px;' />
                        </td>

                        <td style='vertical-align:middle;
                                   font-size:26px;
                                   font-weight:700;
                                   color:#222;
                                   text-align:left;'>
                            Verifique o seu e-mail
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        <!-- SUBTITULO -->
        <tr>
            <td align='center' style='font-size:16px;color:#555;padding-bottom:25px;'>
                Utilize o seguinte código para concluir o registo:
            </td>
        </tr>

        <!-- CÓDIGO -->
        <tr>
            <td align='center'>
                <div style='background:#f0f0f0;padding:18px 0;border-radius:10px;
                            width:260px;margin:auto;font-size:36px;font-weight:700;
                            letter-spacing:10px;color:#222;'>
                    {code}
                </div>
            </td>
        </tr>

        <!-- INFO -->
        <tr>
            <td align='center' style='padding-top:30px;font-size:14px;color:#777;'>
                Este código expira em 10 minutos.
            </td>
        </tr>

        <!-- DIVISOR -->
        <tr>
            <td style='padding-top:30px;'>
                <hr style='border:none;border-top:1px solid #eee;'>
            </td>
        </tr>

        <!-- FOOTER -->
        <tr>
            <td align='center' style='padding-top:10px;font-size:12px;color:#999;'>
                © {DateTime.Now.Year} AutoHub — Todos os direitos reservados.
            </td>
        </tr>

    </table>

</td></tr>
</table>

</body>
</html>";
        }

        // RESET PASSWORD TEMPLATE
        public static string ResetPasswordCode(string code)
        {
            return $@"
<!DOCTYPE html>
<html>
<body style='margin:0;padding:0;background:#f5f5f5;font-family:Arial,Helvetica,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' style='padding:40px 0;'>
<tr><td align='center'>

    <table width='600' cellpadding='0' cellspacing='0'
        style='background:#ffffff;border-radius:14px;padding:40px;
               box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

        <!-- HEADER -->
        <tr>
            <td align='center' style='padding-bottom:20px;'>
                <table cellpadding='0' cellspacing='0' style='margin:auto;'>
                    <tr>
                        <td>
                            <img src='cid:autohub-logo'
                                 width='44'
                                 height='44'
                                 style='vertical-align:middle;margin-right:12px;' />
                        </td>

                        <td style='vertical-align:middle;
                                   font-size:26px;
                                   font-weight:700;
                                   color:#222;'>
                            Recuperar Password
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        <!-- TEXT -->
        <tr>
            <td align='center' style='font-size:16px;color:#555;padding-bottom:25px;'>
                Usa o seguinte código para redefinir a tua password:
            </td>
        </tr>

        <!-- CODE BOX -->
        <tr>
            <td align='center'>
                <div style='background:#f0f0f0;
                            padding:18px 0;
                            border-radius:10px;
                            width:260px;
                            margin:auto;
                            font-size:36px;
                            font-weight:700;
                            letter-spacing:10px;
                            color:#222;'>
                    {code}
                </div>
            </td>
        </tr>

        <!-- SPACING -->
        <tr><td style='padding-top:25px;color:#aaa;font-size:13px;' align='center'>
            Código válido por 10 minutos.
        </td></tr>

    </table>

</td></tr>
</table>

</body>
</html>";
        }

        public static string AdminAccountCreated(string username, string password)
        {
            return $@"
<div style='background:#f8fafc; padding:48px 16px;'>

    <div style='
        background:#ffffff;
        border-radius:18px;
        padding:36px;
        max-width:520px;
        margin:0 auto;
        font-family:Inter, Arial, sans-serif;
        color:#0f172a;
    '>

        <img src='cid:autohub-logo' style='height:36px; margin-bottom:20px;' />

        <h2 style='margin:0 0 10px 0;'>Conta de Administrador criada</h2>

        <p style='color:#475569; font-size:15px;'>
            A tua conta de administrador foi criada com sucesso.
        </p>

        <div style='
            background:#f1f5f9;
            border-radius:12px;
            padding:16px;
            margin:24px 0;
            font-size:14px;
        '>
            <strong>Username:</strong> {username}<br />
            <strong>Password:</strong> {password}
        </div>

        <p style='font-size:13px; color:#64748b;'>
            Por motivos de segurança, recomenda-se a alteração da password após o primeiro login.
        </p>

        <hr style='border:none; border-top:1px solid #e5e7eb; margin:28px 0;' />

        <p style='font-size:12px; color:#94a3b8; text-align:center;'>
            © 2025 AutoHub — Todos os direitos reservados
        </p>

    </div>

</div>";
    }

}
}

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
<body style='font-family:Arial;background:#f5f5f5;padding:30px'>

    <table align='center' width='500'
           style='background:white;border-radius:12px;padding:30px'>

        <tr>
            <td align='center' style='font-size:26px;font-weight:bold;color:#111'>
                Recuperar Password
            </td>
        </tr>

        <tr>
            <td style='padding:20px 0;font-size:16px;text-align:center;color:#333'>
                Usa o seguinte código para redefinir a tua password:
            </td>
        </tr>

        <tr>
            <td align='center'>
                <div style='font-size:40px;font-weight:bold;letter-spacing:8px;
                             background:#f0f0f0;padding:15px;border-radius:8px;color:#111'>
                    {code}
                </div>
            </td>
        </tr>

    </table>

</body>
</html>";
        }
    }
}

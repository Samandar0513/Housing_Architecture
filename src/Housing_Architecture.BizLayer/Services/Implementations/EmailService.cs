using Housing_Architecture.BizLayer.Common;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailConfiguration> config, ILogger<EmailService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public async Task<bool> SendOtpAsync(string toEmail, string otp)
    {
        try
        {
            using var client = new SmtpClient(_config.SmtpServer, _config.Port)
            {
                EnableSsl = _config.EnableSsl,
                Credentials = new NetworkCredential(_config.Username, _config.Password)
            };

            var message = new MailMessage
            {
                From = new MailAddress(_config.DefaultFromEmail, _config.DefaultFromName),
                Subject = "HOUSING: OTP Verification Code",
                Body = GenerateBody(otp),
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
            return true;
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError(smtpEx, "SMTP error sending email to {Email}: {StatusCode} - {Message}",
                toEmail, smtpEx.StatusCode, smtpEx.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", toEmail);
            return false;
        }
    }

    private string GenerateBody(string otp, string? userName = null)
    {
        var displayName = string.IsNullOrWhiteSpace(userName) ? "Hurmatli foydalanuvchi" : $"Hurmatli {WebUtility.HtmlEncode(userName)}";
        var siteUrl = _config.SiteUrl ?? "https://housing.example";
        var logoUrl = _config.LogoUrl ?? "https://housing.example/assets/logo.png";

        var sb = new StringBuilder();
        sb.AppendLine("<!doctype html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset='utf-8' />");
        sb.AppendLine("  <meta name='viewport' content='width=device-width, initial-scale=1' />");
        sb.AppendLine("  <title>HOUSING - Tasdiqlash kodi</title>");
        sb.AppendLine("  <style>");
        sb.AppendLine("    body { background:#f4f6f8; font-family:'Segoe UI',Roboto,Arial; margin:0; padding:0; }");
        sb.AppendLine("    .container { max-width:600px; margin:30px auto; background:#ffffff; border-radius:10px; box-shadow:0 4px 12px rgba(0,0,0,0.1); overflow:hidden; }");
        sb.AppendLine("    .header { background:linear-gradient(90deg,#0d6efd,#6610f2); color:white; padding:25px; text-align:center; }");
        sb.AppendLine("    .header h1 { margin:0; font-size:22px; }");
        sb.AppendLine("    .content { padding:25px; color:#333; }");
        sb.AppendLine("    .otp { background:#f1f5f9; border-radius:8px; padding:15px 20px; text-align:center; font-size:26px; letter-spacing:5px; font-weight:700; margin:25px auto; width:fit-content; }");
        sb.AppendLine("    .footer { background:#f8fafc; text-align:center; padding:18px; font-size:13px; color:#666; }");
        sb.AppendLine("  </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("  <div class='container'>");
        sb.AppendLine("    <div class='header'>");
        sb.AppendLine("      <h1>HOUSING saytiga xush kelibsiz!</h1>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div class='content'>");
        sb.AppendLine($"      <h2>{displayName},</h2>");
        sb.AppendLine("      <p>Bizning HOUSING platformasida ro'yxatdan o'tganingiz uchun tashakkur.</p>");
        sb.AppendLine("      <p>Iltimos, quyidagi tasdiqlash kodini kiritib, email manzilingizni tasdiqlang:</p>");
        sb.AppendLine($"      <div class='otp'>{WebUtility.HtmlEncode(otp)}</div>");
        sb.AppendLine("      <p style='text-align:center;'>Kod 5 daqiqa davomida amal qiladi. Uni hech kim bilan ulashmang.</p>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div class='footer'>");
        sb.AppendLine($"      &copy; {DateTime.UtcNow.Year} HOUSING. Barcha huquqlar himoyalangan.");
        sb.AppendLine("    </div>");
        sb.AppendLine("  </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }
}

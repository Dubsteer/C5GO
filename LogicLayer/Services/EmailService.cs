using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace LogicLayer.Services
{
    public class EmailService
    {
        private readonly string smtpServer;
        private readonly int smtpPort;
        private readonly string smtpUser;
        private readonly string smtpPassword;
        private readonly string fromEmail;
        private readonly string baseUrl;

        public EmailService(IConfiguration configuration)
        {
            smtpServer = GetRequiredSetting(configuration, "EmailSettings:SmtpServer");
            smtpUser = GetRequiredSetting(configuration, "EmailSettings:Username");
            smtpPassword = GetRequiredSetting(configuration, "EmailSettings:Password");
            fromEmail = GetRequiredSetting(configuration, "EmailSettings:SenderEmail");
            baseUrl = GetRequiredSetting(configuration, "AppSettings:AuthUrl").TrimEnd('/');

            if (!int.TryParse(configuration["EmailSettings:Port"], out smtpPort))
            {
                throw new InvalidOperationException("SMTP port is not configured correctly.");
            }
        }

        private static string GetRequiredSetting(IConfiguration configuration, string key)
        {
            var value = configuration[key];
            return string.IsNullOrWhiteSpace(value)
                ? throw new InvalidOperationException($"Configuration value '{key}' is missing.")
                : value;
        }

        public async Task SendVerificationEmail(string toEmail, string token)
        {
            var verifyLink = $"{baseUrl}/VerifyEmail?token={token}";

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("C5GO Platform", fromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Verify your email - C5G0";

            email.Body = new TextPart("plain")
            {
                Text =
                    "Dobro došao na C5G0!\n\n" +
                    "Molimo te da verifikuješ svoj email klikom na link ispod:\n\n" +
                    verifyLink + "\n\n" +
                    "Ako nisi kreirao nalog, slobodno ignoriši ovaj email."
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

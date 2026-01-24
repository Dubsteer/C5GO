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
            smtpServer = configuration["SMTP_HOST"];
            smtpUser = configuration["SMTP_USER"];
            smtpPassword = configuration["SMTP_PASS"];
            fromEmail = configuration["SMTP_FROM"];
            baseUrl = configuration["APP_BASE_URL"]?.TrimEnd('/');

            if (!int.TryParse(configuration["SMTP_PORT"], out smtpPort))
            {
                throw new Exception("SMTP_PORT is not configured correctly.");
            }

            // FAIL FAST – profesionalni standard
            if (string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(smtpUser) ||
                string.IsNullOrWhiteSpace(smtpPassword) ||
                string.IsNullOrWhiteSpace(fromEmail) ||
                string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new Exception("Email environment variables are not configured properly.");
            }
        }

        public async Task SendVerificationEmail(string toEmail, string token)
        {
            var verifyLink = $"{baseUrl}/VerifyEmail?token={token}";

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("C5GO", fromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Verify your email";

            email.Body = new TextPart("plain")
            {
                Text =
                    "Dobro dosao nefilu u C5G0!\n\n" +
                    "Molim te verifikuj svoj email tako sto ces da kliknes na link ispod:\n\n" +
                    verifyLink + "\n\n" +
                    "Ako nisi kreirao ovaj nalog, mozes da ignorises ovaj email."
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

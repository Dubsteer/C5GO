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
            smtpPort = int.Parse(configuration["SMTP_PORT"]);
            smtpUser = configuration["SMTP_USER"];
            smtpPassword = configuration["SMTP_PASS"];
            fromEmail = configuration["SMTP_FROM"];
            baseUrl = configuration["APP_BASE_URL"];

            // FAIL FAST – profesionalni standard
            if (string.IsNullOrEmpty(smtpServer) ||
                string.IsNullOrEmpty(smtpUser) ||
                string.IsNullOrEmpty(smtpPassword) ||
                string.IsNullOrEmpty(fromEmail) ||
                string.IsNullOrEmpty(baseUrl))
            {
                throw new Exception("Email ENV variables are not configured properly.");
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
                    "Welcome to C5GO!\n\n" +
                    "Please verify your email by clicking the link below:\n\n" +
                    verifyLink + "\n\n" +
                    "If you did not create this account, you can ignore this email."
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

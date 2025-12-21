using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;

namespace LogicLayer.Services
{
    public class EmailService
    {
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int smtpPort = 587;
        private readonly string smtpUser = "dovlalegenda@gmail.com";
        private readonly string smtpPassword = "iqye ezyx uyrb jhnf";
        private readonly string fromEmail = "dovlalegenda@gmail.com";

        public async Task SendVerificationEmail(string toEmail, string token)
        {
            var verifyLink =
    $"https://membraneless-untropically-porsha.ngrok-free.dev/VerifyEmail?token={token}";

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

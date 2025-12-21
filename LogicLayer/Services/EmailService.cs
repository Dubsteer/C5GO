using System.Net;
using System.Net.Mail;

namespace LogicLayer.Services
{
    public class EmailService
    {
        private const string SMTP_HOST = "sandbox.smtp.mailtrap.io";
        private const int SMTP_PORT = 2525;

        private const string SMTP_USERNAME = "b098ce3727adb5";
        private const string SMTP_PASSWORD = "3791440113e285";

        private const string FROM_EMAIL = "no-reply@c5go.dev";

        public void SendVerificationEmail(string toEmail, string token)
        {
            var verifyLink = $"https://localhost:7026/VerifyEmail?token={token}";


            var message = new MailMessage();
            message.From = new MailAddress(FROM_EMAIL, "C5GO");
            message.To.Add(toEmail);
            message.Subject = "Verify your email";
            message.Body =
                "Welcome to C5GO!\n\n" +
                "Please verify your email by clicking the link below:\n\n" +
                verifyLink + "\n\n" +
                "If you did not create this account, you can ignore this email.";

            var smtp = new SmtpClient(SMTP_HOST, SMTP_PORT)
            {
                Credentials = new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD),
                EnableSsl = true
            };

            smtp.Send(message);
        }
    }
}

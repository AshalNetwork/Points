
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using SimpleCrm.VM;
using SimpleCrm.Services;


namespace SimpleCrm.Services
{
    public class EmailService(EmailSettings emailSettings) : IEmailService
    {
        private readonly EmailSettings _emailSettings = emailSettings;

        public async Task SendEmailAsync(List<string> to, string subject, string html, string from = null)
        {
            // create message
            var receivers = to.Select(x => MailboxAddress.Parse(x));
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _emailSettings.EmailFrom));
            email.To.AddRange(receivers);
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.SslOnConnect);
            await smtp.AuthenticateAsync(_emailSettings.EmailFrom, _emailSettings.SmtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
